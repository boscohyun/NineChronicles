using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using Nekoyume.Move;
using Newtonsoft.Json;
using Planetarium.Crypto.Extension;
using Planetarium.Crypto.Keys;
using Planetarium.SDK.Action;
using Planetarium.SDK.Address;
using UnityEngine;
using UnityEngine.Networking;

namespace Nekoyume.Network.Agent
{
    [Serializable]
    internal class Response
    {
        public ResultCode result = ResultCode.ERROR;
        public List<Move.Move> moves;
    }

    public class Agent
    {
        public event EventHandler<Move.Move> DidReceiveAction;
        private readonly string apiUrl;
        private readonly PrivateKey privateKey;
        private float interval;
        private OrderedDictionary moves;
        public byte[] UserAddress => privateKey.ToAddress();
        public List<Move.Move> requestedMoves;

        private static JsonConverter moveJsonConverter = new Move.JSONConverter();
        public Agent(string apiUrl, PrivateKey privateKey, float interval = 1.0f)
        {
            if (string.IsNullOrEmpty(apiUrl))
            {
                throw new ArgumentException("apiUrl should not be empty or null.", nameof(apiUrl));
            }

            this.apiUrl = apiUrl;
            this.privateKey = privateKey;
            this.interval = interval;
            this.moves = new OrderedDictionary();
            this.requestedMoves = new List<Move.Move>();
        }

        public void Send(Move.Move move)
        {
            move.Sign(privateKey);
            requestedMoves.Add(move);
        }

        public IEnumerable<Move.Move> Moves => moves.Values.Cast<Move.Move>();

        public IEnumerator SendAll()
        {
            while (true)
            {
                yield return new WaitForSeconds(interval);
                var chunks = new List<Move.Move>(requestedMoves);
                requestedMoves.Clear();

                foreach (var m in chunks)
                {
                    yield return SendMove(m);
                }
            }
        }

        private IEnumerator SendMove(Move.Move move)
        {
            var serialized = JsonConvert.SerializeObject(move, moveJsonConverter);
            var url = string.Format("{0}/moves", apiUrl);
            var request = new UnityWebRequest(url, "POST");
            var payload = Encoding.UTF8.GetBytes(serialized);

            request.uploadHandler = new UploadHandlerRaw(payload);
            request.downloadHandler = new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (!request.isDone || request.isHttpError)
            {
                // FIXME implement better retry logic.(e.g. jitter)
                yield return SendMove(move);
            }
        }

        public IEnumerator Listen()
        {
            long? lastBlockOffset = null;

            while (true)
            {
                yield return new WaitForSeconds(interval);
                var url = string.Format(
                    "{0}/users/0x{1}/moves/", apiUrl, privateKey.ToAddress().Hex()
                );

                if (lastBlockOffset.HasValue)
                {
                    url += string.Format("?block_offset={0}", lastBlockOffset);
                }
                var www = UnityWebRequest.Get(url);
                yield return www.SendWebRequest();
                if (!www.isNetworkError)
                {
                    var jsonPayload = www.downloadHandler.text;
                    var response = JsonConvert.DeserializeObject<Response>(jsonPayload, moveJsonConverter);
                    foreach (var move in response.moves)
                    {
                        moves[move.Id] = move;
                        DidReceiveAction?.Invoke(this, move);
                        lastBlockOffset = move.BlockId;
                    }
                }
            }
        }
    }
}
