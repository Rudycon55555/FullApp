<h1>Apple adoption profile for FullApp</h1>

<h2>1. Scope and goals</h2>
<p>
This document defines the <strong>Apple Adoption Profile</strong> for the FullApp archive format. It describes how Apple platforms (macOS, iOS, iPadOS, tvOS, visionOS, and watchOS) can recognize, validate, sandbox, notarize, and launch <code>.fullapp</code> archives as first-class application containers, alongside existing native formats such as <code>.app</code>, <code>.ipa</code>, and <code>.pkg</code>.
</p>
<p>
The goal of this profile is to:
</p>
<ul>
  <li><strong>Align FullApp with Apple’s security and privacy model</strong>, including notarization and sandboxing.</li>
  <li><strong>Define platform-specific expectations</strong> for metadata, signing, entitlements, and store integration.</li>
  <li><strong>Enable native-feeling installation and launch flows</strong> without traditional installers.</li>
</ul>

<h2>2. Platform integration overview</h2>

<h3>2.1 Recognizing FullApp archives</h3>
<p>
Apple platforms recognize FullApp archives as files with extension <code>.fullapp</code> and a valid ZIP/ZIP64 structure. The system may:
</p>
<ul>
  <li><strong>Associate a uniform type identifier (UTI)</strong> (for example, <code>com.fullapp.archive</code>) with the <code>.fullapp</code> extension.</li>
  <li><strong>Integrate with Finder and Files</strong> to display the app icon from <code>Describe/Icon.png</code> and metadata from <code>Describe/Metadata.json</code>.</li>
  <li><strong>Offer an “Open with FullApp Launcher” action</strong> that invokes the platform’s FullApp runtime.</li>
</ul>

<h3>2.2 Distribution channels</h3>
<p>
FullApp archives can be distributed through:
</p>
<ul>
  <li><strong>App Store / Mac App Store</strong>, where the store treats the <code>.fullapp</code> as the primary payload.</li>
  <li><strong>Notarized direct downloads</strong> on macOS, where a FullApp-aware notarization service inspects and verifies the package before user installation.</li>
  <li><strong>Enterprise and internal distribution</strong> via MDM, where FullApp archives are deployed as managed application bundles.</li>
</ul>
<p>
In all cases, the platform enforces its existing policies (review, signing, entitlements), with this profile defining how those policies apply to FullApp archives.</p>

<h2>3. Metadata and manifest requirements</h2>

<h3>3.1 Required files for Apple profile</h3>
<p>
In addition to the core FullApp specification, Apple platforms require:
</p>
<ul>
  <li><strong><code>Describe/Metadata.json</code></strong> with Apple-specific fields present.</li>
  <li><strong><code>Describe/FullAppData.toml</code></strong> declaring runtime requirements and security flags.</li>
  <li><strong><code>Describe/Icon.png</code></strong> for all GUI applications.</li>
</ul>

<h3>3.2 Apple-specific metadata extensions</h3>
<p>
The <code>Describe/Metadata.json</code> file may define an <code>"apple_profile"</code> object with fields that map to Apple’s platform features:
</p>
<pre>
"apple_profile": {
  "bundle_identifier": "com.example.app",
  "team_id": "ABCDE12345",
  "entitlements_profile": "standard",
  "notarization_required": true,
  "store_eligible": true,
  "supported_apple_platforms": [
    "macos",
    "ios",
    "ipados"
  ]
}
</pre>
<p>
The <code>bundle_identifier</code> must match Apple’s bundle ID rules. <code>team_id</code> refers to the Apple Developer Program team identifier associated with the signing identity.</p>

<h3>3.3 FullAppData.toml Apple mapping</h3>
<p>
The <code>Describe/FullAppData.toml</code> may define an <code>[apple]</code> section that refines platform-specific behavior:
</p>
<pre>
[apple]
sandbox_profile = "default"
requires_notarization = true
allow_in_place_execution = false
preferred_install_location = "user"
</pre>
<p>
If <code>allow_in_place_execution</code> is <code>false</code>, the launcher should treat the <code>.fullapp</code> as a source bundle and install it into a managed application container (for example, inside <code>/Applications</code> or a per-user app directory on macOS).</p>

<h2>4. Security, signing, and sandboxing</h2>

<h3>4.1 Code signing model</h3>
<p>
Apple platforms require that executable content within <code>Program.bnry</code> and other code-carrying files be signed according to platform rules. The <code>Describe/Metadata.json</code> <code>"signing"</code> object provides hints:
</p>
<pre>
"signing": {
  "signature_format": "apple-code-sign",
  "public_key_hint": "Developer ID Application: Example Corp (ABCDE12345)"
}
</pre>
<p>
The launcher and notarization services use this information to verify that:
</p>
<ul>
  <li><strong>The archive integrity</strong> matches the declared hash in <code>"integrity"</code>.</li>
  <li><strong>The embedded code signatures</strong> inside the bundle are valid.</li>
  <li><strong>The signer identity</strong> aligns with the <code>team_id</code> and bundle identifier.</li>
</ul>

<h3>4.2 Mapping permissions to entitlements</h3>
<p>
FullApp’s <code>"permissions"</code> array and <code>[security]</code> section in <code>FullAppData.toml</code> must map to Apple entitlements. For example:
</p>
<pre>
"permissions": [
  {
    "name": "network",
    "description": "Allows outbound HTTP(S) connections."
  }
]
</pre>
<p>
and:
</p>
<pre>
[security]
requires_network = true
requires_filesystem = true
requires_user_input = true
</pre>
<p>
The Apple launcher translates these into a platform-specific entitlements file that is enforced by the sandbox. Apps requesting unsupported or disallowed combinations may be rejected at review or launch time.</p>

<h3>4.3 Notarization and review</h3>
<p>
For macOS:
</p>
<ul>
  <li><strong>Notarization services</strong> inspect <code>.fullapp</code> archives using the defined layout and metadata.</li>
  <li><strong>Automated tooling</strong> checks for malware, invalid signing, and policy violations.</li>
  <li><strong>Gatekeeper</strong> blocks execution of FullApp packages that lack valid notarization, unless the user explicitly overrides policies.</li>
</ul>
<p>
For other Apple OSes, equivalent store review processes may apply before users can install and launch FullApp-based apps.</p>

<h2>5. Installation and launch behavior</h2>

<h3>5.1 Installation flows</h3>
<p>
Apple platforms may support multiple installation modes:
</p>
<ul>
  <li><strong>In-place execution</strong>: The launcher runs the app directly from the <code>.fullapp</code> archive, suitable for temporary or portable use.</li>
  <li><strong>Managed install</strong>: The system copies or expands the archive into a controlled app container directory and tracks it as an installed application.</li>
</ul>
<p>
For store and enterprise environments, the managed install mode is recommended and may be required.</p>

<h3>5.2 Launcher responsibilities on Apple platforms</h3>
<p>
An Apple-compatible FullApp launcher must:
</p>
<ul>
  <li><strong>Validate the archive structure</strong> and ensure Apple-required metadata is present.</li>
  <li><strong>Perform or delegate signing and notarization checks</strong> before first launch.</li>
  <li><strong>Map security declarations to entitlements</strong> and apply the appropriate sandbox profile.</li>
  <li><strong>Launch <code>Program.bnry</code></strong> in the correct platform container, with the configured environment and working directory.</li>
</ul>

<h2>6. Compatibility and evolution</h2>
<p>
The Apple Adoption Profile is versioned alongside the core FullApp specification. New profile versions may:
</p>
<ul>
  <li><strong>Add optional metadata fields</strong> for new Apple platform capabilities.</li>
  <li><strong>Refine security rules</strong> to match updated sandbox and privacy requirements.</li>
  <li><strong>Introduce additional installation modes</strong> or distribution patterns.</li>
</ul>
<p>
Launchers and tools must treat unknown Apple-specific fields as optional and ignore them safely unless explicitly stated otherwise.</p>
