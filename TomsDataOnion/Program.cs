var fileText = await File.ReadAllTextAsync("payload.txt");
var payload = fileText.GetPayload();

//Stage 1
var layer1Text = payload.DecodeASCII85String();
File.WriteAllText("Layers/Layer1.txt", layer1Text);

var layer1Payload = layer1Text.GetPayload();
var layer2Bytes = layer1Payload.DecodeASCII85Bytes();
layer2Bytes.FlipAndRotateBits();
File.WriteAllText("Layers/Layer2.txt", ASCIIEncoding.ASCII.GetString(layer2Bytes));
