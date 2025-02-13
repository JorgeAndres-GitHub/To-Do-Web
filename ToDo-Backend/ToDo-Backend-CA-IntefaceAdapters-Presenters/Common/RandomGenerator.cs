namespace ToDo_Backend_FrameworksDrivers_API.Services.Common
{
    public static class RandomGenerator
    {
        public static string GenerateRandomString(int size)
        {
            var random = new Random();
            var chars = "ABCDEFGHIJKLMNÑOPQRSTUVWXYZabcdefghijklmnñopqrstuvwxyz$#-_.";

            return new string(Enumerable.Repeat(chars, size)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        } 
    }
}
