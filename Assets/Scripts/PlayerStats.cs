public static class PlayerStats
{
    private static int coins;

    static PlayerStats()
    {
        coins = 0;
    }

    public static int GetCoins()
    {
        return coins;
    }

    public static void AddCoins(int value)
    {
        coins += value;
    }

    public static void SpendCoins(int value)
    {
        coins -= value;
    }
}
