using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Unicode;

Console.WriteLine("Add package to repo, Start!");

var targetVpmRepoJsonFilePath = args[0];
Console.WriteLine(targetVpmRepoJsonFilePath);

var input = Environment.GetEnvironmentVariable("Assets");
var tag = Environment.GetEnvironmentVariable("Tag");
if (input is null)
{
    Console.WriteLine("EnvironmentVariable:Assets が存在しない");
    Environment.ExitCode = 1;
    return;
}
if (tag is null)
{
    Console.WriteLine("EnvironmentVariable:Tag が存在しない");
    Environment.ExitCode = 1;
    return;
}

Console.WriteLine("---");
using var httpClient = new HttpClient();
var assets = JsonDocument.Parse(input);




var releasePackageZipPostFix = tag + ".zip";
var ghReleasePackageZip = assets.RootElement.EnumerateArray().First(i => i.GetProperty("name").GetString()?.EndsWith(releasePackageZipPostFix) ?? false);
var packageZipURL = ghReleasePackageZip.GetProperty("browser_download_url").GetString();




var ghReleasePackageJson = assets.RootElement.EnumerateArray().First(i => i.GetProperty("name").GetString() is "package.json");
var ghReleasePackageJsonURL = ghReleasePackageJson.GetProperty("browser_download_url").GetString();
var addSourcePackageJsonRequest = await httpClient.GetAsync(ghReleasePackageJsonURL);
Console.WriteLine(ghReleasePackageJsonURL);

if (addSourcePackageJsonRequest is null)
{
    Console.WriteLine("package.json の取得に失敗");
    Environment.ExitCode = 1;
    return;
}

var addSourcePackageJson = JsonNode.Parse(await addSourcePackageJsonRequest.Content.ReadAsStringAsync());
if (addSourcePackageJson is null) { Console.WriteLine("package.json がうまく取得できませんでした"); Environment.ExitCode = 1; return; }

var addSourcePackageVersion = addSourcePackageJson["version"]?.GetValue<string>();
if (addSourcePackageVersion is null) { Console.WriteLine("package.json から PackageVersion の取得に失敗"); Environment.ExitCode = 1; return; }

var addSourcePackageID = addSourcePackageJson["name"]?.GetValue<string>();
if (addSourcePackageID is null) { Console.WriteLine("package.json から Package ID の取得に失敗"); Environment.ExitCode = 1; return; }

addSourcePackageJson["url"] = packageZipURL;
Console.WriteLine(addSourcePackageID);
Console.WriteLine(addSourcePackageVersion);
Console.WriteLine(packageZipURL);
// Console.WriteLine(addSourcePackageJson);





var vpmRepoJsonString = await File.ReadAllTextAsync(targetVpmRepoJsonFilePath);
var vpmRepoJson = JsonNode.Parse(vpmRepoJsonString);

if (vpmRepoJson is null) { Console.WriteLine("vpm repository json がうまく取得できませんでした"); Environment.ExitCode = 1; return; }

var vpmRepoPackages = vpmRepoJson["packages"];
if (vpmRepoPackages is null) { vpmRepoJson["packages"] = vpmRepoPackages = new JsonObject(); }

var addDestinationPackage = vpmRepoPackages[addSourcePackageID];
if (addDestinationPackage is null) { vpmRepoPackages[addSourcePackageID] = addDestinationPackage = new JsonObject(); }

var addDestinationPackageVersions = addDestinationPackage["versions"];
if (addDestinationPackageVersions is null) { addDestinationPackage["versions"] = addDestinationPackageVersions = new JsonObject(); }

addDestinationPackageVersions[addSourcePackageVersion] = addSourcePackageJson;



var serializeOption = new JsonSerializerOptions()
{
    WriteIndented = true,
    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping,
};
var addedVpmJsonString = JsonSerializer.Serialize(vpmRepoJson, serializeOption) + "\n";
await File.WriteAllTextAsync(targetVpmRepoJsonFilePath, addedVpmJsonString);

Console.WriteLine("---");
