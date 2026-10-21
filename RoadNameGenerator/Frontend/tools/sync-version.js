// ModVersion.props is the only editable source of the release version.
// Uses Node built-ins only, so this also runs before npm ci.
const fs = require("fs");
const path = require("path");

function syncVersion(projectRoot = path.resolve(__dirname, "../..")) {
    const source = fs.readFileSync(path.join(projectRoot, "ModVersion.props"), "utf8");
    const values = [...source.matchAll(/<RoadNameGeneratorVersion>\s*([^<]+?)\s*<\/RoadNameGeneratorVersion>/g)];
    if (values.length !== 1) throw new Error("ModVersion.props must contain exactly one RoadNameGeneratorVersion.");
    const version = values[0][1].trim();
    if (!/^(0|[1-9]\d*)\.(0|[1-9]\d*)\.(0|[1-9]\d*)$/.test(version) ||
        version.split(".").some(value => Number(value) > 65534)) {
        throw new Error("Invalid mod version: use Major.Minor.Patch, each number between 0 and 65534.");
    }

    const uiFile = path.join(projectRoot, "Frontend", "mod.json");
    const publishFile = path.join(projectRoot, "Properties", "PublishConfiguration.xml");
    const ui = fs.readFileSync(uiFile, "utf8");
    const publish = fs.readFileSync(publishFile, "utf8");
    JSON.parse(ui.replace(/^\uFEFF/, ""));
    const uiPattern = /("version"\s*:\s*")[^"]*(")/g;
    const publishPattern = /(<ModVersion\b[^>]*\bValue\s*=\s*")[^"]*(")/g;
    if ([...ui.matchAll(uiPattern)].length !== 1 ||
        [...publish.matchAll(publishPattern)].length !== 1) {
        throw new Error("Expected exactly one version in mod.json and one ModVersion Value in PublishConfiguration.xml.");
    }

    // Validate both files before modifying either; preserve descriptions,
    // changelogs, BOMs, indentation and line endings byte-for-byte.
    const outputs = [
        [uiFile, ui, ui.replace(uiPattern, (_, before, after) => before + version + after)],
        [publishFile, publish, publish.replace(publishPattern, (_, before, after) => before + version + after)]
    ];
    for (const [file, oldText, newText] of outputs) {
        if (oldText !== newText) fs.writeFileSync(file, newText, "utf8");
    }
    return version;
}

if (require.main === module) {
    try {
        console.log(`Road Name Generator version: ${syncVersion()}`);
    } catch (error) {
        console.error(error.message);
        process.exitCode = 1;
    }
}

module.exports = { syncVersion };
