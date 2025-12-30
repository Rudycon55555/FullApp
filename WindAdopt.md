<h1>Windows adoption profile for FullApp</h1>

<h2>1. Scope and goals</h2>
<p>
This document defines the <strong>Windows Adoption Profile</strong> for the FullApp archive format. It describes how Windows can recognize, validate, secure, and launch <code>.fullapp</code> archives as first-class application containers, alongside existing formats such as <code>.exe</code>, <code>.appx</code>, and <code>.msix</code>.
</p>
<p>
The goals of this profile are to:
</p>
<ul>
  <li><strong>Integrate FullApp with Windows shell, security, and deployment tools</strong>.</li>
  <li><strong>Provide a safer alternative to traditional installers</strong> while remaining familiar to Windows users.</li>
  <li><strong>Enable system administrators and developers</strong> to adopt FullApp without losing existing Windows capabilities.</li>
</ul>

<h2>2. Platform integration overview</h2>

<h3>2.1 Recognizing FullApp archives</h3>
<p>
Windows recognizes FullApp archives as files with extension <code>.fullapp</code> and ZIP/ZIP64 structure. Integration points include:
</p>
<ul>
  <li><strong>File association</strong> so that double-clicking a <code>.fullapp</code> file launches a FullApp-capable runtime.</li>
  <li><strong>Explorer integration</strong> to show app icons from <code>Describe/Icon.png</code> and key metadata such as name and version.</li>
  <li><strong>Context menu actions</strong> such as “Run FullApp”, “Install FullApp”, and “Verify FullApp”.</li>
</ul>

<h3>2.2 Distribution channels</h3>
<p>
Windows can distribute FullApp archives through:
</p>
<ul>
  <li><strong>Direct downloads</strong> from websites, internal servers, or file shares.</li>
  <li><strong>Microsoft Store</strong>, where a <code>.fullapp</code> is used as the underlying payload format for an app package.</li>
  <li><strong>Enterprise deployment tools</strong> such as Intune, Configuration Manager, or custom scripts, which can treat <code>.fullapp</code> files as managed content.</li>
</ul>

<h2>3. Metadata and manifest requirements</h2>

<h3>3.1 Required files for Windows profile</h3>
<p>
In addition to the core FullApp requirements, Windows launchers expect:
</p>
<ul>
  <li><strong><code>Describe/Metadata.json</code></strong> including Windows-specific fields.</li>
  <li><strong><code>Describe/FullAppData.toml</code></strong> defining the entry point, runtimes, and security flags.</li>
  <li><strong><code>Describe/Icon.png</code></strong> for GUI applications.</li>
</ul>

<h3>3.2 Windows-specific metadata extensions</h3>
<p>
<code>Describe/Metadata.json</code> may define a <code>"windows_profile"</code> object that maps FullApp metadata onto Windows concepts:
</p>
<pre>
"windows_profile": {
  "product_id": "ExampleApp",
  "publisher": "Example Corp",
  "publisher_id": "CN=Example Corp, O=Example Corp",
  "install_scope": "per-user",
  "supports_silent_install": true,
  "supports_shortcuts": true
}
</pre>
<p>
The <code>install_scope</code> may be <code>"per-user"</code> or <code>"per-machine"</code>, guiding how the launcher installs the app when a managed install mode is used.</p>

<h3>3.3 FullAppData.toml Windows mapping</h3>
<p>
The <code>Describe/FullAppData.toml</code> file may contain a <code>[windows]</code> section to configure behavior:
</p>
<pre>
[windows]
create_start_menu_entry = true
create_desktop_shortcut = false
install_location = "AppDataLocal"
elevate_for_machine_install = true
</pre>
<p>
Launchers should use these values when performing an install-style operation, while still enforcing Windows security policies and user consent dialogs.</p>

<h2>4. Security, signing, and policy</h2>

<h3>4.1 Code signing model</h3>
<p>
Windows supports Authenticode and related mechanisms for signing executables and installers. In the FullApp context:
</p>
<ul>
  <li><strong>The <code>.fullapp</code> archive</strong> can be signed as a single file (e.g., using catalog signing or external metadata).</li>
  <li><strong>Individual binaries inside the archive</strong> may also be signed according to normal Windows rules.</li>
</ul>
<p>
The <code>"signing"</code> object in <code>Describe/Metadata.json</code> may advertise the intended Windows signing model:
</p>
<pre>
"signing": {
  "signature_format": "windows-authenticode",
  "public_key_hint": "CN=Example Corp, O=Example Corp"
}
</pre>
<p>
Launchers and security tools validate the archive integrity, verify code signatures, and surface the publisher identity to users and administrators.</p>

<h3>4.2 Mapping permissions to Windows policy</h3>
<p>
FullApp permissions (in JSON and TOML) must map to Windows capabilities and policy controls. For example, requesting network and filesystem access may:
</p>
<ul>
  <li><strong>Trigger Windows Defender Application Control policies</strong> if configured.</li>
  <li><strong>Influence AppLocker or enterprise rules</strong> that allow or block certain apps.</li>
  <li><strong>Drive user-facing consent prompts</strong> in managed environments.</li>
</ul>
<p>
Enterprise administrators can use the structured metadata to pre-approve or restrict FullApp packages without reverse engineering installers.</p>

<h3>4.3 Defender and reputation services</h3>
<p>
Windows Defender and SmartScreen can integrate with FullApp by:
</p>
<ul>
  <li><strong>Scanning the <code>.fullapp</code> archive</strong> using the standardized directory layout.</li>
  <li><strong>Using <code>Describe/Metadata.json</code></strong> to establish application identity and publisher reputation.</li>
  <li><strong>Applying reputation checks</strong> before allowing a FullApp to run or install.</li>
</ul>

<h2>5. Installation and launch behavior</h2>

<h3>5.1 Execution modes</h3>
<p>
Windows launchers may support:
</p>
<ul>
  <li><strong>Run-only</strong>: Execute <code>Program.bnry</code> directly from the archive without installation, useful for portable apps.</li>
  <li><strong>Install-then-run</strong>: Expand or copy the contents into a per-user or per-machine directory, create shortcuts, and register the app with the system.</li>
</ul>
<p>
The default mode can depend on <code>windows_profile</code> fields, user policy, and administrator configuration.</p>

<h3>5.2 Launcher responsibilities on Windows</h3>
<p>
A Windows-compatible FullApp launcher must:
</p>
<ul>
  <li><strong>Validate archive integrity</strong> and check for required metadata and manifests.</li>
  <li><strong>Respect Windows security policies</strong>, including execution restrictions, SmartScreen, and Defender.</li>
  <li><strong>Optionally create shell integrations</strong> such as Start Menu entries, taskbar pins, and file associations, based on metadata.</li>
  <li><strong>Launch <code>Program.bnry</code></strong> with the correct working directory, arguments, and environment variables.</li>
</ul>

<h2>6. Compatibility and evolution</h2>
<p>
The Windows Adoption Profile evolves with the core FullApp spec and Windows platform capabilities. Future versions may:
</p>
<ul>
  <li><strong>Introduce tighter integration with MSIX</strong> and Windows Package Manager.</li>
  <li><strong>Add richer policy hooks</strong> for enterprises managing large fleets of FullApp-based applications.</li>
  <li><strong>Define advanced install scenarios</strong> such as shared runtimes or side-by-side deployments.</li>
</ul>
<p>
Launchers and tools should handle unknown Windows-specific fields gracefully, maintaining compatibility across versions.</p>
