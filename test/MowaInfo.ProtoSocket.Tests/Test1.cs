using System;
using System.Threading.Tasks;
using Xunit;

namespace MowaInfo.ProtoSocket.Tests
{
    public class Test1
    {
        private Task Call()
        {
            TaskCompletionSource<int> tcs1 = new TaskCompletionSource<int>();
            Task.Delay(TimeSpan.FromSeconds(5)).ContinueWith(_ => tcs1.SetException(new Exception()));
            tcs1.Task.ContinueWith(_ => Console.WriteLine("111"));
            return tcs1.Task;
        }

        [Fact]
        public void TaskCompletionSourceTest()
        {
            Call();
            Console.ReadLine();
        }
    }
}
