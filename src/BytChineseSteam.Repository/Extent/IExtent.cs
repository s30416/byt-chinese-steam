using System.Text.Json.Nodes;

namespace BytChineseSteam.Repository.Extent;

public interface IExtent
{
    string Name => throw new NotImplementedException();
    void LoadFromJsonArray(JsonArray element);
    JsonNode ToJson();
}