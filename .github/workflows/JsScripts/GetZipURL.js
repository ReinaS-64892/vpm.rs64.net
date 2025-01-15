const { Assets, Tag } = process.env

const zipEndName = Tag + ".zip"

console.log(zipEndName);

for (const asset of JSON.parse(Assets)) {
    if (asset.name.endsWith(zipEndName)) {
        return asset.browser_download_url;
    }
}

/**
 * assets: [
 *  {
 *  ~~~
 *    name: 'package.json',
 *  ~~~
 *    browser_download_url: 'https://github.com/ReinaS-64892/TexTransTool/releases/download/v0.9.0-beta.5/package.json'
 *  },
 *  {
 *  ~~~
 *    name: 'tex-trans-tool-v0.9.0-beta.5.zip',
 *  ~~~
 *    browser_download_url: 'https://github.com/ReinaS-64892/TexTransTool/releases/download/v0.9.0-beta.5/tex-trans-tool-v0.9.0-beta.5.zip'
 *  }
 *],
 */
