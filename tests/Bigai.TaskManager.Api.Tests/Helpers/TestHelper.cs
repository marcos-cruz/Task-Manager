using System.Text;
using System.Text.Json;

namespace Bigai.TaskManager.Api.Tests.Helpers
{
    public static class TestHelper
    {
        private const string JsonMediaType = "application/json";

        public static StringContent GetJsonStringContent<T>(T model)
        {
            return new(JsonSerializer.Serialize(model), Encoding.UTF8, JsonMediaType);
        }
    }
}