var fileText = await File.ReadAllTextAsync("payload.txt");
var payload = fileText.GetPayload();

//Stage 1
var stage1 = payload.DecodeASCII85();
Console.WriteLine(stage1);