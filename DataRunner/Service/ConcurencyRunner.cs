using MySQLDemo.Framework.Infrastructure;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DataRunner.Service
{
    public class ConcurencyRunner : IConcurencyRunner
    {
        private IRedisRepository _redisRepository;
        Task[] tasks = new Task[100];
        bool isContinue = true;
        public ConcurencyRunner(IRedisRepository redisRepository)
        {
            _redisRepository = redisRepository;
        }

        Hashtable table = new Hashtable();

        public void Do()
        {
            for (int i = 0; i < 100; i++)
            {
                tasks[i] = Task.Run(async () =>
                {
                    while (isContinue)
                    {
                        var val = await _redisRepository.Incr("hello");
                        Console.WriteLine(val);

                        table.Add(val, val);

                    }

                });
            }
        }

        public void Stop()
        {
            isContinue = true;
            Task.WaitAll(tasks);

        }
    }
}
