<h1>FullApp archive format specification</h1>

<h2>1. Scope and goals</h2>
<p>
FullApp is a portable application container format represented as a ZIP archive with the <code>.fullapp</code> extension. A FullApp package encapsulates executable code, runtime components, assets, metadata, configuration, and legal information in a standardized directory layout to enable consistent loading, execution, update, and inspection across platforms.
</p>

<h2>2. Top-level characteristics</h2>
<ul>
<li><strong>Extension:</strong> <code>.fullapp</code></li>
<li><strong>Container format:</strong> ZIP archive, standard ZIP64-compatible structure</li>
<li><strong>Root directory name:</strong> Not required; the archive root is treated as the application root</li>
<li><strong>Primary entry file:</strong> <code>Program.bnry</code> at the archive root</li>
<li><strong>Character encoding for text files:</strong> UTF-8 without BOM unless stated otherwise for a specific file</li>
<li><strong>Path separator inside archive:</strong> <code>/</code></li>
<li><strong>Case sensitivity:</strong> Paths are case sensitive inside the archive; consumers must treat directory and file names exactly as specified in this document</li>
</ul>

<h2>3. Required and optional entries</h2>
<ul>
<li><strong>Required top-level entries:</strong>
<ul>
<li><code>Program.bnry</code></li>
<li><code>Describe/README.md</code></li>
<li><code>Describe/Metadata.json</code></li>
<li><code>Describe/FullAppData.toml</code></li>
</ul>
</li>
<li><strong>Conditionally required entries:</strong>
<ul>
<li><code>Describe/Icon.png</code> is required for any app that declares a graphical user interface.</li>
<li><code>Describe/Legal/</code> must exist if the package includes third-party components with license obligations or if the author provides explicit license terms.</li>
</ul>
</li>
<li><strong>Optional entries:</strong> All other paths defined in this specification are optional unless explicitly marked as required or conditionally required.</li>
<li><strong>Prohibited entries:</strong> No paths outside the defined directory layout may be treated as authoritative by conforming loaders, though they may be ignored or surfaced as user-visible extras.</li>
</ul>

<h2>4. Directory layout</h2>
<pre>
Program.bnry
OtherCode/
Resources/
Runtimes/
Assets/
Static/
Dynamics/
Link-Against-Targets/
Other/
Describe/
README.md
Instructions.yaml
Configs.yaml
Metadata.json
HumanMetadata.yaml
FullAppData.toml
Icon.png
Legal/
Other/
Other/
</pre>

<h2>5. Path-by-path definition</h2>

<h3>5.1 <code>Program.bnry</code></h3>
<ul>
<li><strong>Location:</strong> Archive root.</li>
<li><strong>Required:</strong> Yes.</li>
<li><strong>Purpose:</strong> Primary executable or entry module loaded by the FullApp runtime.</li>
<li><strong>Format:</strong> Opaque <code>.bnry</code> container interpreted by the platform-specific FullApp launcher. The spec does not mandate an instruction set or language.</li>
<li><strong>Responsibilities:</strong>
<ul>
<li>Acts as the starting point of execution.</li>
<li>May dynamically load additional <code>.bnry</code> modules from <code>OtherCode/</code>, <code>Resources/Runtimes/</code>, or <code>Resources/Link-Against-Targets/</code>.</li>
<li>May consult files under <code>Describe/</code> for configuration, metadata, and instructions.</li>
</ul>
</li>
</ul>

<h3>5.2 <code>OtherCode/</code></h3>
<ul>
<li><strong>Location:</strong> Archive root.</li>
<li><strong>Required:</strong> No.</li>
<li><strong>Contents:</strong> Additional <code>.bnry</code> modules or supporting code artifacts used by <code>Program.bnry</code>.</li>
<li><strong>Substructure:</strong> Unconstrained; any subdirectories and filenames are allowed, but FullApp-aware tools should not rely on a specific pattern beyond the root directory name.</li>
<li><strong>Usage:</strong> Import, plugin, or feature modules that are tightly coupled to the main program.</li>
</ul>

<h3>5.3 <code>Resources/</code></h3>
<ul>
<li><strong>Location:</strong> Archive root.</li>
<li><strong>Required:</strong> No.</li>
<li><strong>Purpose:</strong> Contains non-primary code dependencies and assets required at runtime.</li>
</ul>

<h4>5.3.1 <code>Resources/Runtimes/</code></h4>
<ul>
<li><strong>Required:</strong> No.</li>
<li><strong>Contents:</strong> Runtime components packaged as <code>.bnry</code> files or other opaque binaries (for example, language runtimes, standard libraries, or interpreters).</li>
<li><strong>Naming:</strong> Any naming scheme is allowed; tooling is expected to use <code>Describe/FullAppData.toml</code> or <code>Describe/Metadata.json</code> to declare and resolve runtime usage.</li>
<li><strong>Isolation:</strong> Runtimes in this directory are considered private to the FullApp package unless explicitly declared as shared.</li>
</ul>

<h4>5.3.2 <code>Resources/Assets/Static/</code></h4>
<ul>
<li><strong>Required:</strong> No.</li>
<li><strong>Contents:</strong> Immutable or versioned assets such as images, audio, fonts, localization files, and other static resources bundled directly into the archive.</li>
<li><strong>File types:</strong> Unrestricted; consumers must treat them as opaque and rely on file extensions or metadata when necessary.</li>
</ul>

<h4>5.3.3 <code>Resources/Assets/Dynamics/</code></h4>
<ul>
<li><strong>Required:</strong> No.</li>
<li><strong>Contents:</strong> Text files (usually <code>.txt</code>, <code>.json</code>, <code>.yaml</code>, or equivalent) that contain URLs or file paths referencing assets that may be downloaded, generated, or mounted at runtime.</li>
<li><strong>Semantics:</strong>
<ul>
<li>Each file in this directory describes one or more dynamic asset sources.</li>
<li>The format of each descriptor must be declared in <code>Describe/FullAppData.toml</code> or <code>Describe/Metadata.json</code>.</li>
<li>Dynamic assets are not required to be present within the archive but must resolve consistently when the FullApp is executed in a supported environment.</li>
</ul>
</li>
</ul>

<h4>5.3.4 <code>Resources/Link-Against-Targets/</code></h4>
<ul>
<li><strong>Required:</strong> No.</li>
<li><strong>Contents:</strong> Libraries and link-time or load-time targets packaged as <code>.bnry</code> or other binary formats to be linked or dynamically loaded by <code>Program.bnry</code>.</li>
<li><strong>Usage:</strong>
<ul>
<li>Static libraries, dynamic libraries, or modular components that extend or support <code>Program.bnry</code>.</li>
<li>Resolution rules, including version constraints and ABI requirements, must be declared via entries in <code>Describe/FullAppData.toml</code> or <code>Describe/Metadata.json</code>.</li>
</ul>
</li>
</ul>

<h4>5.3.5 <code>Resources/Other/</code></h4>
<ul>
<li><strong>Required:</strong> No.</li>
<li><strong>Purpose:</strong> Catch-all directory for resource types not covered by other subdirectories under <code>Resources/</code>.</li>
<li><strong>Semantics:</strong> No standardized semantics; content is considered opaque to generic FullApp loaders and is interpreted only by the application.</li>
</ul>

<h3>5.4 <code>Describe/</code></h3>
<ul>
<li><strong>Location:</strong> Archive root.</li>
<li><strong>Required:</strong> Yes, with specific required files.</li>
<li><strong>Purpose:</strong> Human-readable and machine-readable metadata, configuration, instructions, and legal documentation.</li>
</ul>

<h4>5.4.1 <code>Describe/README.md</code></h4>
<ul>
<li><strong>Required:</strong> Yes.</li>
<li><strong>Format:</strong> CommonMark-flavored Markdown.</li>
<li><strong>Purpose:</strong> High-level description of the app including overview, features, usage summary, system requirements, and maintainer information.</li>
<li><strong>Content guidelines:</strong>
<ul>
<li>Must contain an <code>#</code>-level heading with the application name as the first heading.</li>
<li>Should include sections for installation/execution, known limitations, and support channels where applicable.</li>
</ul>
</li>
</ul>

<h4>5.4.2 <code>Describe/Instructions.yaml</code></h4>
<ul>
<li><strong>Required:</strong> No.</li>
<li><strong>Format:</strong> YAML.</li>
<li><strong>Purpose:</strong> Structured instructions for installation, migration, uninstallation, and environment-specific notes.</li>
<li><strong>Recommended structure:</strong>
<pre>
version: 1
instructions:

id: default-run
summary: Run the app with default settings
steps:

"Invoke the FullApp launcher with this file."

"Follow any prompts shown by the launcher."
platform_overrides:

platform: "windows"
instructions_ref: default-run
</pre>
</li>
</ul>

<h4>5.4.3 <code>Describe/Configs.yaml</code></h4>
<ul>
<li><strong>Required:</strong> No.</li>
<li><strong>Format:</strong> YAML.</li>
<li><strong>Purpose:</strong> Declarative configuration defaults and schema hints for application-level configuration values.</li>
<li><strong>Typical content:</strong>
<ul>
<li>Configuration keys, default values, types, and validation rules.</li>
<li>Information on whether specific configuration can be overridden by environment variables, command-line arguments, or external files.</li>
</ul>
</li>
</ul>

<h4>5.4.4 <code>Describe/Metadata.json</code></h4>
<ul>
<li><strong>Required:</strong> Yes.</li>
<li><strong>Format:</strong> JSON.</li>
<li><strong>Purpose:</strong> Machine-readable core metadata used by launchers, stores, and management tools.</li>
<li><strong>Top-level keys (required unless marked optional):</strong></li>
</ul>
<pre>
{
"format": "FullApp",
"format_version": "1.0.0",
"app_id": "com.example.app",
"name": "Example App",
"version": "1.0.0",
"summary": "Short one-line description.",
"description": "Longer plain-text or Markdown-compatible description.",
"license": "MIT",
"authors": [
{
"name": "Example Author",
"email": "author@example.com",
"url": "https://example.com"
}
],
"homepage": "https://example.com/app",
"source_code": "https://example.com/app/source",
"documentation": "https://example.com/app/docs",
"categories": ["utility", "development"],
"platforms": [
{
"os": "windows",
"arch": "x86_64",
"min_version": "10"
}
],
"entry": {
"path": "Program.bnry",
"arguments": [],
"environment": {}
},
"runtimes": [
{
"id": "net.fullapp.runtime.example",
"version": "1.0.0",
"path": "Resources/Runtimes/example-runtime.bnry"
}
],
"permissions": [
{
"name": "network",
"description": "Allows outbound HTTP(S) connections."
}
],
"integrity": {
"hash_algorithm": "SHA256",
"hash_of_archive": "hex-encoded-hash"
},
"signing": {
"signature_format": "opaque",
"public_key_hint": "optional-key-id-or-url"
},
"extras": {}
}
</pre>
<p>
Keys marked as optional in the narrative are not required to appear in the JSON, but if present they must follow the specified type conventions.
</p>

<h4>5.4.5 <code>Describe/HumanMetadata.yaml</code></h4>
<ul>
<li><strong>Required:</strong> No.</li>
<li><strong>Format:</strong> YAML.</li>
<li><strong>Purpose:</strong> Redundant, human-centric metadata intended for editors, reviewers, and maintainers; may mirror or expand upon <code>Metadata.json</code> in a more narrative style.</li>
<li><strong>Relation to <code>Metadata.json</code>:</strong> If both exist, <code>Metadata.json</code> is authoritative for machine consumers; inconsistencies should be treated as warnings by tools.</li>
</ul>

<h4>5.4.6 <code>Describe/FullAppData.toml</code></h4>
<ul>
<li><strong>Required:</strong> Yes.</li>
<li><strong>Format:</strong> TOML.</li>
<li><strong>Purpose:</strong> Low-level, strongly typed description of the FullApp package structure and runtime requirements; serves as the core manifest file for loaders.</li>
<li><strong>Recommended sections:</strong></li>
</ul>
<pre>
[fullapp]
format = "FullApp"
format_version = "1.0.0"

[app]
id = "com.example.app"
name = "Example App"
version = "1.0.0"
kind = "gui" # or "cli", "service"

[entry]
path = "Program.bnry"
args = []
working_directory = "."
stdout_mode = "inherit" # or "capture"
stderr_mode = "inherit"

[[runtime]]
id = "net.fullapp.runtime.example"
path = "Resources/Runtimes/example-runtime.bnry"
min_version = "1.0.0"
max_version = "2.0.0"

[[library]]
id = "com.example.lib"
path = "Resources/Link-Against-Targets/example-lib.bnry"
required = true

[assets.static]
root = "Resources/Assets/Static"

[assets.dynamic]
root = "Resources/Assets/Dynamics"
descriptor_format = "yaml"

[security]
requires_network = true
requires_filesystem = true
requires_user_input = true
</pre>

<h4>5.4.7 <code>Describe/Icon.png</code></h4>
<ul>
<li><strong>Required:</strong> Required for GUI-class apps; optional otherwise.</li>
<li><strong>Format:</strong> PNG image.</li>
<li><strong>Recommended sizes:</strong> Square dimension, such as 512x512, with downscaling handled by consumers.</li>
</ul>

<h4>5.4.8 <code>Describe/Legal/</code></h4>
<ul>
<li><strong>Required:</strong> Required when the application has explicit license terms or includes third-party components requiring attribution or license bundling; optional otherwise.</li>
<li><strong>Common contents:</strong>
<ul>
<li><code>LICENSE</code> or <code>LICENSE.txt</code> for main license.</li>
<li><code>THIRD_PARTY.md</code> for third-party notices.</li>
<li>Any additional legal, patent, or export control information.</li>
</ul>
</li>
</ul>

<h4>5.4.9 <code>Describe/Other/</code></h4>
<ul>
<li><strong>Required:</strong> No.</li>
<li><strong>Purpose:</strong> Additional descriptive artifacts, design documents, or human-targeted notes that do not belong in other standardized subdirectories.</li>
</ul>

<h3>5.5 <code>Other/</code></h3>
<ul>
<li><strong>Location:</strong> Archive root.</li>
<li><strong>Required:</strong> No.</li>
<li><strong>Purpose:</strong> Sandbox area for arbitrary data, experiments, or tooling artifacts not recognized by the FullApp specification.</li>
<li><strong>Semantics:</strong> Generic loaders must ignore this directory for execution purposes, although they may surface it as part of an “extra contents” view.</li>
</ul>

<h2>6. Execution model</h2>
<ul>
<li><strong>Launcher responsibilities:</strong>
<ul>
<li>Validate the ZIP archive structure and ensure that required files exist.</li>
<li>Parse <code>Describe/FullAppData.toml</code> and <code>Describe/Metadata.json</code>.</li>
<li>Resolve and provision any declared runtimes in <code>Resources/Runtimes/</code> or from host-level runtime providers.</li>
<li>Prepare the execution environment (working directory, arguments, environment variables) as declared.</li>
<li>Load and execute <code>Program.bnry</code> as the primary entry point.</li>
</ul>
</li>
<li><strong>Program responsibilities:</strong>
<ul>
<li>Handle configuration, assets, and dynamic resources per its own logic.</li>
<li>Respect declared capabilities and permissions where supported by the runtime or sandbox.</li>
</ul>
</li>
</ul>

<h2>7. Versioning and compatibility</h2>
<ul>
<li><strong>Format version:</strong> The <code>format_version</code> field in both <code>Metadata.json</code> and <code>FullAppData.toml</code> indicates the FullApp specification version targeted by the package.</li>
<li><strong>Application version:</strong> The <code>version</code> field in <code>Metadata.json</code> and <code>[app]</code> section in the TOML identifies the application’s own release version.</li>
<li><strong>Backward compatibility:</strong> Launchers targeting a given major format version must not assume compatibility with packages specifying a higher major version.</li>
<li><strong>Forward compatibility:</strong> Packages may declare lower format versions to run on older launchers but must not rely on features introduced in newer versions.</li>
</ul>

<h2>8. Security and integrity</h2>
<ul>
<li><strong>Integrity metadata:</strong> The <code>integrity</code> object in <code>Metadata.json</code> may declare a hash algorithm and hash of the entire archive for verification purposes.</li>
<li><strong>Signing metadata:</strong> The <code>signing</code> object in <code>Metadata.json</code> may describe signature formats and key hints; the actual signature container and verification process are left to platform profiles built on top of FullApp.</li>
<li><strong>Permissions model:</strong> The <code>permissions</code> array in <code>Metadata.json</code> and the <code>[security]</code> section in <code>FullAppData.toml</code> declare requested capabilities; enforcement is platform dependent.</li>
<li><strong>Sandboxing:</strong> The FullApp specification is agnostic to any specific sandbox model but is designed so that a sandbox can grant or deny capabilities based on declared metadata.</li>
</ul>

<h2>9. Error handling and validation rules</h2>
<ul>
<li><strong>Missing <code>Program.bnry</code>:</strong> The launcher must treat the archive as invalid and refuse to run it.</li>
<li><strong>Missing <code>Describe/FullAppData.toml</code> or <code>Describe/Metadata.json</code>:</strong> The launcher must treat the archive as invalid and refuse to run it.</li>
<li><strong>Malformed metadata:</strong> If <code>Metadata.json</code> or <code>FullAppData.toml</code> is syntactically invalid, the launcher must treat the archive as invalid.</li>
<li><strong>Non-critical inconsistencies:</strong> Inconsistencies between human-oriented files (such as <code>README.md</code> and <code>HumanMetadata.yaml</code>) and machine-oriented metadata must be surfaced as warnings, but the launcher may still execute the app if core requirements are met.</li>
<li><strong>Unrecognized paths:</strong> Files and directories not mentioned in this specification must not affect the launcher’s decision to run the app but may be ignored or displayed as extra content.</li>
</ul>

<h2>10. Reserved and naming conventions</h2>
<ul>
<li><strong>Reserved path prefixes:</strong> Any future expansion of this specification must use additional subdirectories under <code>Resources/</code> and <code>Describe/</code> or new top-level directories defined in updated versions of this document.</li>
<li><strong>Case sensitivity:</strong> The exact capitalization of <code>Program.bnry</code>, <code>OtherCode/</code>, <code>Resources/</code>, <code>Describe/</code>, and <code>Other/</code> is reserved and must be followed exactly.</li>
<li><strong>File extensions:</strong> The <code>.bnry</code> extension is reserved for opaque binary payloads intended to be interpreted or executed by the FullApp runtime or by runtimes contained within the package.</li>
</ul>
