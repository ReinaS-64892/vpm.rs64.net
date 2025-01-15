const { PackageJson , ReplaceURL } = process.env

let jsonObj = JSON.parse(PackageJson);
jsonObj.url = ReplaceURL;
return JSON.stringify(jsonObj);
