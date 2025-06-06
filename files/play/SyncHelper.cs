public static class SyncHelper
{
    public static T Sync<T>(this Task<T> task)
    {
        return task.GetAwaiter().GetResult();
    }

    public static void Sync(this Task task)
    {
        task.GetAwaiter().GetResult();
    }
}