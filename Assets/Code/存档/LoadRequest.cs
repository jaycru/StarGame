public static class LoadRequest
{
    public static bool isLoadingSave;
    public static string saveId;

    public static void RequestLoad(string id)
    {
        isLoadingSave = true;
        saveId = id;
    }

    public static void Clear()
    {
        isLoadingSave = false;
        saveId = null;
    }
}
