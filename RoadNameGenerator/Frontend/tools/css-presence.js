const { Compilation, sources } = require("webpack");

exports.CSSPresencePlugin = class CSSPresencePlugin {
    apply(compiler) {
        compiler.hooks.thisCompilation.tap("CSSPresencePlugin", compilation => {
            compilation.hooks.processAssets.tap({
                name: "CSSPresencePlugin",
                stage: Compilation.PROCESS_ASSETS_STAGE_ADDITIONS,
            }, () => {
                const hasCSS = Object.keys(compilation.assets).some(file => file.endsWith(".css"));
                for (const chunk of compilation.chunks) {
                    for (const file of chunk.files) {
                        if (!file.endsWith(".mjs")) continue;
                        const source = compilation.getAsset(file).source.source();
                        compilation.updateAsset(file, new sources.RawSource(source.replace(
                            "export {", `const hasCSS = ${hasCSS}; export { hasCSS, `)));
                    }
                }
            });
        });
    }
};
