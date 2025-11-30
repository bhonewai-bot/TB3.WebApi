using Newtonsoft.Json.Linq;

namespace TB3.Models;

public class ResponseModel
{
    public Object Return(ReturnModel model)
    {
        JObject jsonObject = new JObject(
            new JProperty("message", model.Message),
            new JProperty("isSuccess", model.IsSuccess),
            new JProperty("data", model.Item is null
                ? model.Item
                : new JObject(
                    new JProperty(model.EnumPos.ToString().ToLower(), JToken.FromObject(model.Item))
                )
            )
        );
        if (model.Count is not null)
        {
            jsonObject.Add(new JProperty("result", model.Count));
        }
        return jsonObject;
    }
}

public class ReturnModel
{
    public string Message { get; set; }
    public int? Count { get; set; }
    public EnumPos EnumPos { get; set; }
    public bool IsSuccess { get; set; }
    public Object? Item { get; set; }
}