﻿using System.Threading;
using System.Threading.Tasks;
using AElf.Cryptography.ECDSA;
using AElf.Kernel.KernelAccount;
using AElf.Kernel.Services;
using AElf.Kernel.TxMemPool;
using Google.Protobuf;
using NLog;
using Xunit;
using Xunit.Frameworks.Autofac;

namespace AElf.Kernel.Tests.TxMemPool
{
    [UseAutofacTestFramework]
    public class TxPoolServiceTest
    {
        private readonly IAccountContextService _accountContextService;
        private readonly ILogger _logger;

        
        public TxPoolServiceTest(IAccountContextService accountContextService, ILogger logger)
        {
            _accountContextService = accountContextService;
            _logger = logger;
        }

        private TxPool GetPool()
        {
            return new TxPool(TxPoolConfig.Default, _logger);
        }

        [Fact]
        public async Task Serialize()
        {
            var tx = Transaction.Parser.ParseFrom(ByteString.FromBase64(
                @"CiIKIKkqNVMSxCWn/TizqYJl0ymJrnrRqZN+W3incFJX3MRIEiIKIIFxBhlGhI1auR05KafXd/lFGU+apqX96q1YK6aiZLMhIgh0cmFuc2ZlcioJCgcSBWhlbGxvOiEAxfMt77nwSKl/WUg1TmJHfxYVQsygPj0wpZ/Pbv+ZK4pCICzGxsZBCBlASmlDdn0YIv6vRUodJl/9jWd8Q1z2ofFwSkEE+PDQtkHQxvw0txt8bmixMA8lL0VM5ScOYiEI82LX1A6oWUNiLIjwAI0Qh5fgO5g5PerkNebXLPDE2dTzVVyYYw=="));
            var pool = GetPool();
            var keypair = new KeyPairGenerator().Generate();
            var poolService = new TxPoolService(pool, _accountContextService);
            poolService.Start();
            await poolService.AddTxAsync(tx);
        }
        
        [Fact]
        public async Task ServiceTest()
        {
            var pool = GetPool();
            
            var poolService = new TxPoolService(pool, _accountContextService);
            poolService.Start();
           
            var addr11 = Hash.Generate();
            var addr12 = Hash.Generate();
            var addr21 = Hash.Generate();
            var addr22 = Hash.Generate();

            var tx1 = TxPoolTest.BuildTransaction();
            var res = await poolService.AddTxAsync(tx1);
            Assert.True(res);
            
            Assert.Equal(1, (int) await poolService.GetWaitingSizeAsync());
            Assert.Equal(0, (int) await poolService.GetExecutableSizeAsync());
            Assert.Equal(1, (int)pool.Size);
            
            var tx2 = TxPoolTest.BuildTransaction();
            res = await poolService.AddTxAsync(tx2);
            
            Assert.True(res);
            Assert.Equal(2, (int) await poolService.GetWaitingSizeAsync());
            Assert.Equal(0, (int) await poolService.GetExecutableSizeAsync());
            Assert.Equal(2, (int)pool.Size);
            
            
            /*var tx3 = new Transaction
            {
                From = addr11,
                To = Hash.Generate(),
                IncrementId = 0
            };
            res = await poolService.AddTxAsync(tx3);
            
            Assert.True(res);
            Assert.Equal(3, (int) await poolService.GetWaitingSizeAsync());
            Assert.Equal(0, (int) await poolService.GetExecutableSizeAsync());
            Assert.Equal(0, (int)pool.Size);
            
            
            var tx4 = new Transaction
            {
                From = addr11,
                To = Hash.Generate(),
                IncrementId = 1
            };
            
            res = await poolService.AddTxAsync(tx4);
            Assert.True(res);
            Assert.Equal(4, (int) await poolService.GetWaitingSizeAsync());
            Assert.Equal(0, (int) await poolService.GetExecutableSizeAsync());
            Assert.Equal(0, (int)pool.Size);
            
            var tx5 = new Transaction
            {
                From = addr11,
                To = Hash.Generate(),
                IncrementId = 2
            };
            res = await poolService.AddTxAsync(tx5);
            Assert.True(res);
            Thread.Sleep(1000);
            Assert.Equal(0, (int) await poolService.GetTmpSizeAsync());
            Assert.Equal(5, (int) await poolService.GetPoolSize());
            Assert.Equal(4, (int) await poolService.GetWaitingSizeAsync());
            Assert.Equal(0, (int) await poolService.GetExecutableSizeAsync());
            
            var tx6 = new Transaction
            {
                From = addr11,
                To = Hash.Generate(),
                IncrementId = 2
            };
            res = await poolService.AddTxAsync(tx6);
            Assert.True(res);
            Assert.Equal(1, (int) await poolService.GetTmpSizeAsync());
            Assert.Equal(4, (int) await poolService.GetWaitingSizeAsync());
            Assert.Equal(0, (int) await poolService.GetExecutableSizeAsync());*/
            
            await poolService.Stop();
            
            var tx7 = new Transaction
            {
                From = addr11,
                To = Hash.Generate(),
                IncrementId = 3
            };
            res = await poolService.AddTxAsync(tx7);
            Assert.False(res);
            
        }
    }
}