var fileText = await File.ReadAllTextAsync("payload.txt");
var payload = fileText.GetPayload();

//Stage 1
var stage1 = payload.DecodeASCII85String();

var stage2Bytes = stage1.DecodeASCII85Bytes();

stage2Bytes.FlipAndRotateBits();
Console.WriteLine(ASCIIEncoding.Default.GetString(stage2Bytes));