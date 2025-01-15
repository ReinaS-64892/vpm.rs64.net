export function Func() {
    const { VPMRepoJson, AddPackageJson } = process.env

    let jsonObj = JSON.parse(VPMRepoJson);
    let addJsonObj = JSON.parse(AddPackageJson);

    jsonObj.packages[addJsonObj.name].versions[addJsonObj.version] = addJsonObj;

    return JSON.stringify(jsonObj, null, 2);
}
