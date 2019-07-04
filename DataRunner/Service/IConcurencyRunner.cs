namespace DataRunner.Service
{
    public interface IConcurencyRunner
    {
        void Do();
        void Stop();
    }
}