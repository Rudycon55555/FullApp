<h1>FullApp File Format Specification</h1>

<p><strong>Version:</strong> 1.0.0<br />
<strong>Status:</strong> Ready<br />
<strong>License:</strong> MIT</p>

<hr />

<h2>1. Overview</h2>

<p>The <code>.fullapp</code> format defines a cross-platform, self-contained application container. A FullApp package is a single compressed ZIP archive that includes binaries, runtimes, assets, metadata, and configuration files required to execute an application on one or more operating systems and CPU architectures.</p>

<p>The goal of the format is to provide a universal, portable application packaging mechanism that:</p>

<ul>
  <li><strong>Decouples</strong> application packaging from any specific operating system’s native installer or bundle mechanism.</li>
  <li><strong>Supports</strong> multiple OS and CPU targets in a single file.</li>
  <li><strong>Allows</strong> applications to run without full extraction of the archive.</li>
  <li><strong>Defines</strong> a clear, stable, and extensible internal directory structure.</li>
</ul>

<p>The FullApp specification is designed to be simple enough for developers to understand and implement, while being structured enough for robust launcher implementations across Linux, Windows, macOS, and other platforms.</p>

<hr />

<h2>2. Container format and file extension</h2>

<ul>
  <li><strong>File extension:</strong> A FullApp package <em>must</em> use the extension <code>.fullapp</code>.</li>
  <li><strong>Container type:</strong> A FullApp package <em>must</em> be a valid ZIP archive.</li>
  <li><strong>Compression:</strong> Any standard ZIP compression method is permitted (e.g., store, deflate).</li>
  <li><strong>Directory structure:</strong> The archive <em>must</em> contain the minimum required paths defined in this document.</li>
</ul>

<p>Launchers must treat the <code>.fullapp</code> extension as an indicator that the file is a candidate FullApp package, and must verify its validity according to the identification rules in Section 3.</p>

<hr />

<h2>3. Identification and validity</h2>

<p>A file is considered a valid FullApp container if and only if all of the following conditions are met:</p>

<ol>
  <li><strong>ZIP validity:</strong> The file is a structurally valid ZIP archive.</li>
  <li><strong>Format descriptor presence:</strong> The archive contains the file:
    <pre><code>Describe/FullAppFormat.txml</code></pre>
  </li>
  <li><strong>Format descriptor contents:</strong> The <code>Describe/FullAppFormat.txml</code> file contains a syntactically valid TXML document with a root <code>&lt;fullapp&gt;</code> element, which includes at least:
    <pre><code>&lt;fullapp&gt;
  version = "1.0"
&lt;/fullapp&gt;
</code></pre>
  </li>
</ol>

<p>If any of these conditions fail, the file must not be treated as a valid FullApp package by a compliant launcher.</p>

<hr />

<h2>4. Directory structure</h2>

<p>A FullApp archive is a ZIP file with a standardized top-level layout. The recommended structure is as follows:</p>

<ul>
  <li><strong>Program.bnry</strong> — the main executable or primary loader binary.</li>
  <li><strong>HelperBinaryFiles/</strong> — optional supporting binaries that may be invoked by the main program.</li>
  <li><strong>Recources/</strong> — non-code resources used by the application, including:
    <ul>
      <li><strong>Runtimes/</strong> — embedded runtimes (for example: Python, Node.js, JVM, Lua, custom interpreters).</li>
      <li><strong>Link-Against-Targets/</strong> — libraries, shared objects, or other binary components required by <code>Program.bnry</code> or helper binaries.</li>
      <li><strong>Assets/</strong> — static and dynamic asset references:
        <ul>
          <li><strong>Static/</strong> — direct asset files (images, sounds, data files, etc.).</li>
          <li><strong>Dynamic/</strong> — text files that contain pointers to static assets or remote URLs.</li>
        </ul>
      </li>
    </ul>
  </li>
  <li><strong>Functions/</strong> — optional predefined callable functions, scripts, or modules that can be invoked by the launcher or the main program.</li>
  <li><strong>Describe/</strong> — metadata and configuration files:
    <ul>
      <li><strong>Metadata.json</strong> — machine-readable application metadata.</li>
      <li><strong>HumanMetadata.yaml</strong> — human-readable metadata (for documentation and display).</li>
      <li><strong>README.txt</strong> — plain-text human description of the package.</li>
      <li><strong>Legal/</strong> — legal and policy documents:
        <ul>
          <li><strong>License.txt</strong></li>
          <li><strong>ToS.txt</strong></li>
          <li><strong>Rules-and-Regulations.txt</strong></li>
          <li><strong>Code-of-Conduct.txt</strong></li>
          <li><strong>Other-Notices/</strong> — additional notices or legal files.</li>
        </ul>
      </li>
      <li><strong>Config.toml</strong> — OS/CPU configuration and runtime selection rules.</li>
      <li><strong>Instructions.yaml</strong> — optional human- or machine-readable instructions.</li>
      <li><strong>Anything-Else/</strong> — developer-defined extras and extension files.</li>
      <li><strong>FullApp.txml</strong> — primary launcher instruction file (entrypoints, permissions, runtime choices, etc.).</li>
      <li><strong>FullAppFormat.txml</strong> — format descriptor and version declaration.</li>
      <li><strong>Icon.png</strong> — application icon (recommended square PNG).</li>
    </ul>
  </li>
  <li><strong>Other/</strong> — optional, arbitrary developer content not covered by the above conventions.</li>
</ul>

<p>Only the presence and contents of <code>Describe/FullAppFormat.txml</code> are required for basic format recognition. However, additional files are required for a package to be considered executable, as defined in Section 5.</p>

<hr />

<h2>5. Required files for executability</h2>

<p>A minimal FullApp package that is intended to be executable by a compliant launcher <em>must</em> include at least the following files:</p>

<ul>
  <li><strong>Program.bnry</strong></li>
  <li><strong>Describe/FullAppFormat.txml</strong></li>
  <li><strong>Describe/FullApp.txml</strong></li>
  <li><strong>Describe/Metadata.json</strong></li>
</ul>

<p>If any of these files are missing, the launcher may treat the package as invalid or non-executable, even if it is technically a valid ZIP archive and contains a <code>FullAppFormat.txml</code> descriptor.</p>

<hr />

<h2>6. TXML specification</h2>

<p>TXML (TOML XML) is a simple, hybrid configuration language used by the FullApp format for structured configuration and launcher instructions. TXML combines:</p>

<ul>
  <li><strong>XML-style elements</strong> for hierarchical structure.</li>
  <li><strong>TOML-style key/value pairs</strong> inside elements for configuration entries.</li>
</ul>

<h3>6.1. Syntax rules</h3>

<ul>
  <li><strong>Elements:</strong> TXML uses XML-style tags:
    <pre><code>&lt;tag&gt;
  ...
&lt;/tag&gt;
</code></pre>
  </li>
  <li><strong>Key/value pairs:</strong> Inside an element, configuration entries use TOML-style syntax:
    <pre><code>key = "value"
another_key = "another value"
</code></pre>
  </li>
  <li><strong>Comments:</strong> Lines beginning with <code>#</code> are comments and extend to the end of the line.</li>
  <li><strong>No attributes:</strong> Opening tags must not contain XML attributes. All configuration is expressed as key/value pairs inside the element body.</li>
  <li><strong>Whitespace:</strong> Whitespace is insignificant except inside quoted strings.</li>
</ul>

<h3>6.2. Example document</h3>

<pre><code>&lt;fullapp&gt;
  id = "com.example.app"
  version = "1.0"

  &lt;entrypoints&gt;
    &lt;entry&gt;
      os = "linux"
      arch = "x86_64"
      binary = "Program.bnry"
    &lt;/entry&gt;

    &lt;entry&gt;
      os = "windows"
      arch = "x86_64"
      binary = "Program.bnry"
    &lt;/entry&gt;
  &lt;/entrypoints&gt;
&lt;/fullapp&gt;
</code></pre>

<hr />

<h2>7. Metadata files</h2>

<h3>7.1. Describe/Metadata.json</h3>

<p><code>Metadata.json</code> is a machine-readable JSON file that describes the application and its supported targets. It should follow strict JSON syntax and is intended for use by launchers, stores, and tooling.</p>

<p>Recommended fields include:</p>

<ul>
  <li><strong>id</strong> — a unique application identifier (for example: <code>"com.example.app"</code>).</li>
  <li><strong>name</strong> — human-readable name of the application.</li>
  <li><strong>version</strong> — application version as a string.</li>
  <li><strong>description</strong> — short description of the application.</li>
  <li><strong>authors</strong> — list of author names or objects containing author information.</li>
  <li><strong>supported_targets</strong> — list of supported OS/CPU targets (for example: <code>"linux-x86_64"</code>, <code>"windows-x86_64"</code>, <code>"macos-arm64"</code>).</li>
</ul>

<h3>7.2. Describe/HumanMetadata.yaml</h3>

<p><code>HumanMetadata.yaml</code> is an optional but recommended YAML 1.2 file intended to be human-readable. It may duplicate or expand upon information in <code>Metadata.json</code>, and can be used by documentation tools, UIs, or human reviewers.</p>

<h3>7.3. Describe/Config.toml</h3>

<p><code>Config.toml</code> is a TOML configuration file that defines OS/CPU-specific rules, runtime selection policies, environment settings, and launcher constraints. It may include, for example:</p>

<ul>
  <li><strong>Per-OS configuration</strong> (Linux, Windows, macOS, etc.).</li>
  <li><strong>Per-architecture configuration</strong> (x86_64, arm64, etc.).</li>
  <li><strong>Runtime selection rules</strong> that decide whether to use embedded runtimes or system runtimes.</li>
  <li><strong>Launcher hints</strong> (for example: logging options, temporary extraction strategies).</li>
</ul>

<h3>7.4. Describe/FullApp.txml</h3>

<p><code>FullApp.txml</code> is the primary launcher instruction file. It is written in TXML and must define how the launcher should execute the application. Typical responsibilities include:</p>

<ul>
  <li><strong>Entrypoints:</strong> Selecting the appropriate program or script for a given OS and CPU architecture.</li>
  <li><strong>Runtime selection:</strong> Deciding whether to use embedded runtimes in <code>Recources/Runtimes/</code> or system-installed runtimes.</li>
  <li><strong>Asset roots:</strong> Declaring locations of static and dynamic assets.</li>
  <li><strong>Permissions:</strong> Declaring requested filesystem, network, and other permissions (interpretation is launcher-dependent).</li>
</ul>

<p>Launchers should treat <code>FullApp.txml</code> as the authoritative source for application execution behavior, overridable only by launcher policy or user configuration.</p>

<hr />

<h2>8. Binaries and helper files</h2>

<h3>8.1. Program.bnry</h3>

<p><code>Program.bnry</code> is the primary binary of the application package. It may be:</p>

<ul>
  <li>A native executable binary (for example: ELF on Linux, PE on Windows, Mach-O on macOS).</li>
  <li>A launcher binary that starts a higher-level runtime (for example: embedded Node.js, Python, or a VM).</li>
</ul>

<p>Unless explicitly overridden in <code>FullApp.txml</code>, a launcher should treat <code>Program.bnry</code> as the default entrypoint, when a suitable OS/CPU match exists.</p>

<h3>8.2. HelperBinaryFiles/</h3>

<p><code>HelperBinaryFiles/</code> is an optional directory containing supporting binaries that may be used by <code>Program.bnry</code> or other components. Launchers must not automatically execute binaries from this directory; their use is controlled by the main program and configuration files.</p>

<hr />

<h2>9. Resources and assets</h2>

<h3>9.1. Recources/Runtimes/</h3>

<p>The <code>Recources/Runtimes/</code> directory contains embedded runtimes such as Python interpreters, Node.js distributions, JVMs, or custom virtual machines. Launchers may choose to:</p>

<ul>
  <li>Use these embedded runtimes when specified by <code>FullApp.txml</code> or <code>Config.toml</code>.</li>
  <li>Fallback to system runtimes when embedded runtimes are absent or unsuitable.</li>
</ul>

<h3>9.2. Recources/Link-Against-Targets/</h3>

<p>The <code>Recources/Link-Against-Targets/</code> directory stores shared libraries or other binary components required by the main program or helper binaries. Launchers may adjust library paths or environment variables to ensure these components are available at run time, according to platform-specific rules.</p>

<h3>9.3. Recources/Assets/</h3>

<p>The <code>Recources/Assets/</code> directory contains application assets in two main forms:</p>

<ul>
  <li><strong>Static/</strong> — direct asset files, such as images, audio, fonts, configuration text files, and other data.</li>
  <li><strong>Dynamic/</strong> — text files that contain pointers to either static assets inside the archive or external URLs.</li>
</ul>

<p>The dynamic asset pointer format is text-based. Example formats include:</p>

<pre><code>static://Recources/Assets/Static/path/to/file
url://https://example.com/resource
</code></pre>

<p>The exact interpretation of these pointers is launcher- or application-defined, but the above forms are recommended conventions.</p>

<hr />

<h2>10. Legal content</h2>

<p>The <code>Describe/Legal/</code> directory may contain legal and policy documents relevant to the application and its distribution. Recommended files include:</p>

<ul>
  <li><strong>License.txt</strong> — the license governing the packaged application.</li>
  <li><strong>ToS.txt</strong> — terms of service, if applicable.</li>
  <li><strong>Rules-and-Regulations.txt</strong> — additional rules or policies.</li>
  <li><strong>Code-of-Conduct.txt</strong> — behavioral expectations for users or contributors.</li>
  <li><strong>Other-Notices/</strong> — any additional legal or informational notices.</li>
</ul>

<p>The FullApp format itself does not enforce any particular legal content or licensing. However, launchers may choose to display or require acceptance of certain files (such as <code>License.txt</code> or <code>ToS.txt</code>) before executing the application, according to launcher policy.</p>

<hr />

<h2>11. Execution model</h2>

<p>A compliant FullApp launcher should implement the following high-level execution model:</p>

<ol>
  <li><strong>Open the archive:</strong> Open the <code>.fullapp</code> ZIP archive. The launcher may operate directly on the archive contents without fully extracting it, unless platform restrictions require extraction.</li>
  <li><strong>Verify format:</strong> Confirm the presence and validity of <code>Describe/FullAppFormat.txml</code> as described in Section 3.</li>
  <li><strong>Parse configuration:</strong>
    <ul>
      <li>Parse <code>Describe/FullApp.txml</code> for entrypoints, runtime settings, permissions, and other instructions.</li>
      <li>Optionally parse <code>Describe/Config.toml</code> for OS/CPU-specific behavior.</li>
      <li>Optionally parse <code>Describe/Metadata.json</code> and <code>Describe/HumanMetadata.yaml</code> for display and tooling.</li>
    </ul>
  </li>
  <li><strong>Select entrypoint:</strong> Choose an appropriate entrypoint based on:
    <ul>
      <li>Current operating system.</li>
      <li>Current CPU architecture.</li>
      <li>Configuration rules in <code>FullApp.txml</code> and <code>Config.toml</code>.</li>
    </ul>
  </li>
  <li><strong>Resolve runtime:</strong> Determine whether to use:
    <ul>
      <li>An embedded runtime in <code>Recources/Runtimes/</code>.</li>
      <li>A system-installed runtime.</li>
      <li>No runtime (for native executables).</li>
    </ul>
  </li>
  <li><strong>Handle assets:</strong> Provide access to static and dynamic assets, preferably via streaming from the archive, with optional temporary extraction when required by the platform.</li>
  <li><strong>Execute:</strong> Launch the appropriate program (usually <code>Program.bnry</code> or another binary specified in <code>FullApp.txml</code>) and manage its lifecycle according to platform conventions.</li>
</ol>

<p>Launchers may implement additional features such as logging, sandboxing, or cached extraction, as long as they do not violate the core expectations of this specification.</p>

<hr />

<h2>12. Security considerations</h2>

<p>The FullApp format itself does not mandate any specific security mechanisms, but launcher implementations should consider the following best practices:</p>

<ul>
  <li><strong>Permission controls:</strong> Implement controls for filesystem access, network access, and other sensitive operations, where supported by the platform.</li>
  <li><strong>Runtime sandboxing:</strong> Use OS-provided sandboxing mechanisms where available.</li>
  <li><strong>Signature verification:</strong> Optionally support cryptographic signatures for <code>.fullapp</code> archives or their contents.</li>
  <li><strong>Integrity checks:</strong> Validate archive integrity (for example, ZIP central directory checks, checksums, or hashes).</li>
  <li><strong>Prompting users:</strong> Inform users when running untrusted or unsigned FullApp packages, according to platform norms.</li>
</ul>

<p>Security policies are launcher- and platform-dependent and may evolve independently of the base format.</p>

<hr />

<h2>13. Versioning</h2>

<p>The FullApp format version is declared in the <code>Describe/FullAppFormat.txml</code> file. A minimal example:</p>

<pre><code>&lt;fullapp&gt;
  version = "1.0"
  min_launcher_version = "1.0"
&lt;/fullapp&gt;
</code></pre>

<ul>
  <li><strong>version</strong> — the version of the FullApp format used by this package.</li>
  <li><strong>min_launcher_version</strong> — the minimum launcher version required to reliably process this package.</li>
</ul>

<p>Launchers must refuse to run packages that specify a format version or minimum launcher version that is not supported, unless they have an explicit compatibility mode that safely handles such packages.</p>

<hr />

<h2>14. Extensibility</h2>

<p>The FullApp format is designed to be extensible without breaking existing tools. Developers may:</p>

<ul>
  <li>Add new directories under <code>Describe/</code>, <code>Recources/</code>, <code>Functions/</code>, or <code>Other/</code>.</li>
  <li>Add new configuration or metadata files, as long as required files remain present and intact.</li>
  <li>Extend <code>FullApp.txml</code>, <code>Config.toml</code>, and other files with additional fields that parsers can safely ignore if unknown.</li>
</ul>

<p>Extensions should be designed to fail gracefully: older launchers should be able to ignore unknown fields or directories without crashing, while newer launchers can take advantage of the extra information.</p>

<hr />

<h2>15. Minimal valid FullApp package</h2>

<p>A minimal <em>valid and executable</em> FullApp package must contain at least the following four files at the specified paths:</p>

<ul>
  <li><strong>Program.bnry</strong></li>
  <li><strong>Describe/FullAppFormat.txml</strong></li>
  <li><strong>Describe/FullApp.txml</strong></li>
  <li><strong>Describe/Metadata.json</strong></li>
</ul>

<p>All other directories and files described in this specification are optional but strongly recommended for real-world applications.</p>

<hr />

<h2>16. Reference implementation and repository</h2>

<p>A reference implementation of the FullApp launcher, packer tools, and example packages may be provided in a public repository, for example:</p>

<p><code>Rudycon55555/FullApp</code> (GitHub)</p>

<p>The reference implementation and this specification may be distributed under the MIT License, allowing developers to adopt, modify, and extend FullApp tooling freely.</p>
