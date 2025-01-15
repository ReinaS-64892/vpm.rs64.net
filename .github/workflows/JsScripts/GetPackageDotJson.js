const { Assets } = process.env

(async () => {

        for (const asset of JSON.parse(Assets)) {
            if (asset.name === "package.json") {
                const res = await fetch(asset.browser_download_url);
                return await res.text();
            }
        }

    })();
