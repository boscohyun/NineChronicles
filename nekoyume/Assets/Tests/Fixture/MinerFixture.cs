using System.Collections;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.IO;
using System.Threading.Tasks;
using Libplanet;
using Libplanet.Action;
using Libplanet.Crypto;
using Libplanet.Net;

using Libplanet.Tx;
using Nekoyume.Action;
using Nekoyume.BlockChain;
using NUnit.Framework;
using UnityEngine;

namespace Tests
{
    [TestFixture]
    public class MinerFixture
    {
        private readonly string _storePath;
        private readonly TestAgent _agent;

        public MinerFixture()
        {
        }
        public MinerFixture(string storeName)
        {
            _storePath = $"{storeName}.ldb";
            const string hex = "02ed49dbe0f2c34d9dff8335d6dd9097f7a3ef17dfb5f048382eebc7f451a50aa1";
            var privateKey = new PrivateKey(ByteUtil.ParseHex(hex));
            if (File.Exists(_storePath))
                File.Delete(_storePath);
            _agent = new TestAgent(privateKey, storeName, new List<Peer>(), new List<IceServer>(),  "", null);
        }

        public IEnumerator CoMine(Transaction<PolymorphicAction<ActionBase>> transaction)
        {
            yield return _agent.CoMine(transaction);
        }

        public void TearDown()
        {
            _agent.TearDown();
            if (File.Exists(_storePath))
                File.Delete(_storePath);
        }

        private class TestAgent: Agent
        {
            public TestAgent(PrivateKey privateKey, string path, IEnumerable<Peer> peers,
                IEnumerable<IceServer> iceServers, string host, int? port)
                : base(privateKey, path, peers, iceServers, host, port)
            {
            }

            public void TearDown()
            {
                _store.Dispose();
            }

            public IEnumerator CoMine(Transaction<PolymorphicAction<ActionBase>> transaction)
            {
                var task = Task.Run(() =>
                    _blocks.StageTransactions(
                        ImmutableHashSet<Transaction<PolymorphicAction<ActionBase>>>.Empty.Add(transaction)
                    )
                );
                yield return new WaitUntil(() => task.IsCompleted);
                var mine = Task.Run(() => _blocks.MineBlock(PrivateKey.PublicKey.ToAddress()));
                yield return new WaitUntil(() => mine.IsCompleted);
                var block = mine.Result;
                AgentController.Agent.AppendBlock(block);
            }
        }
    }
}
