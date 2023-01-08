var fileText = await File.ReadAllTextAsync("payload.txt");
var payload = fileText.GetPayload();

var outputDirectory = "Layers";
if (Directory.Exists(outputDirectory)) { Directory.Delete(outputDirectory, true); }
Directory.CreateDirectory(outputDirectory);

//Stage 1
var layer1Text = payload.DecodeASCII85String();
File.WriteAllText($"{outputDirectory}/Layer1.txt", layer1Text);

var layer1Payload = layer1Text.GetPayload();
var layer2Bytes = layer1Payload.DecodeASCII85Bytes();
layer2Bytes.FlipAndRotateBits();
var layer2Text = ASCIIEncoding.ASCII.GetString(layer2Bytes);
await File.WriteAllTextAsync($"{outputDirectory}/Layer2.txt", layer2Text);

var layer2Payload = layer2Text.GetPayload();
var layer3Bytes = layer2Payload.DecodeASCII85Bytes();
var layer3Text = ASCIIEncoding.ASCII.GetString(layer3Bytes.RemoveParityFailures());
await File.WriteAllTextAsync($"{outputDirectory}/Layer3.txt", layer3Text);

var layer4Bytes = layer3Text.GetPayload().DecodeASCII85Bytes().DecryptLayer4();
var layer4Text = ASCIIEncoding.ASCII.GetString(layer4Bytes);
await File.WriteAllTextAsync($"{outputDirectory}/Layer4.txt", layer4Text);

var layer5Payload = layer4Text.GetPayload().DecodeASCII85Bytes();
var layer5Bytes = layer5Payload.ReadValidUDP();
var layer5Text = ASCIIEncoding.ASCII.GetString(layer5Bytes);
await File.WriteAllTextAsync($"{outputDirectory}/Layer5.txt", layer5Text);

var layer6Payload = layer5Text.GetPayload().DecodeASCII85Bytes();
var layer6Bytes = layer6Payload.DecryptLayer6();
var layer6Text = ASCIIEncoding.ASCII.GetString(layer6Bytes);
await File.WriteAllTextAsync($"{outputDirectory}/Layer6.txt", layer6Text);