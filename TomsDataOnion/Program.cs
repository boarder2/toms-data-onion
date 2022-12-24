var fileText = await File.ReadAllTextAsync("payload.txt");
var payload = fileText.GetPayload();

//Stage 1
var layer1Text = payload.DecodeASCII85String();
File.WriteAllText("Layers/Layer1.txt", layer1Text);

var layer1Payload = layer1Text.GetPayload();
var layer2Bytes = layer1Payload.DecodeASCII85Bytes();
layer2Bytes.FlipAndRotateBits();
var layer2Text = ASCIIEncoding.ASCII.GetString(layer2Bytes);
await File.WriteAllTextAsync("Layers/Layer2.txt", layer2Text);

var layer2Payload = layer2Text.GetPayload();
var layer3Bytes = layer2Payload.DecodeASCII85Bytes();
var layer3Text = ASCIIEncoding.ASCII.GetString(layer3Bytes.RemoveParityFailures());
await File.WriteAllTextAsync("Layers/Layer3.txt", layer3Text);

var layer4Bytes = layer3Text.GetPayload().DecodeASCII85Bytes().DecryptLayer4();
var layer4Text = ASCIIEncoding.ASCII.GetString(layer4Bytes);
await File.WriteAllTextAsync("Layers/Layer4.txt", layer4Text);