<h1>Easy Linux adoption profile for FullApp</h1>

<h2>1. Scope and goals</h2>
<p>
This document defines the <strong>Easy Linux Adoption Profile</strong> for the FullApp archive format. It focuses on how Linux distributions, desktop environments, and community tooling can quickly adopt <code>.fullapp</code> archives as a simple, cross-distro application container without heavy central control.
</p>
<p>
The goals of this profile are to:
</p>
<ul>
  <li><strong>Make it trivial for Linux users to run FullApp-based applications</strong> on common distributions.</li>
  <li><strong>Integrate FullApp smoothly with desktop environments</strong> (GNOME, KDE, etc.) and common package ecosystems.</li>
  <li><strong>Encourage community-driven tooling</strong> rather than a single centralized authority.</li>
</ul>

<h2>2. Platform integration overview</h2>

<h3>2.1 Recognizing FullApp archives</h3>
<p>
Linux systems recognize FullApp archives as files with extension <code>.fullapp</code> and ZIP/ZIP64 structure. Desktop environments and file managers can:
</p>
<ul>
  <li><strong>Associate a MIME type</strong>, such as <code>application/x-fullapp</code>, with the <code>.fullapp</code> extension.</li>
  <li><strong>Display metadata and icons</strong> by reading <code>Describe/Metadata.json</code> and <code>Describe/Icon.png</code>.</li>
  <li><strong>Offer a “Run FullApp” action</strong> that uses a system-wide or user-level launcher.</li>
</ul>

<h3>2.2 Distribution channels</h3>
<p>
On Linux, FullApp archives can be distributed by:
</p>
<ul>
  <li><strong>Direct downloads</strong> from project websites and Git hosting platforms.</li>
  <li><strong>Linux-specific app catalogs</strong> that index <code>.fullapp</code> files and provide metadata and reviews.</li>
  <li><strong>Package managers</strong> that treat FullApp archives as payloads or wrap them in minimal native packages.</li>
</ul>

<h2>3. Metadata and manifest expectations</h2>

<h3>3.1 Core requirements</h3>
<p>
Linux launchers rely on the core FullApp specification:
</p>
<ul>
  <li><strong><code>Program.bnry</code></strong> at the archive root.</li>
  <li><strong><code>Describe/Metadata.json</code></strong> with basic app metadata (ID, name, version, summary, categories, platforms).</li>
  <li><strong><code>Describe/FullAppData.toml</code></strong> specifying entry, runtimes, and security flags.</li>
  <li><strong><code>Describe/Icon.png</code></strong> for GUI apps to integrate with desktop menus.</li>
</ul>

<h3>3.2 Linux-specific metadata extensions</h3>
<p>
<code>Describe/Metadata.json</code> may contain a <code>"linux_profile"</code> object for desktop integration:
</p>
<pre>
"linux_profile": {
  "desktop_entry": true,
  "desktop_categories": ["Utility", "Development"],
  "preferred_terminal": false,
  "compatible_distros": ["ubuntu", "debian", "fedora", "arch"]
}
</pre>
<p>
When <code>desktop_entry</code> is <code>true</code>, launchers or helper tools may generate a <code>.desktop</code> file to integrate the app into application menus.</p>

<h3>3.3 FullAppData.toml Linux mapping</h3>
<p>
<code>Describe/FullAppData.toml</code> may include a <code>[linux]</code> section:
</p>
<pre>
[linux]
requires_x11 = false
requires_wayland = true
create_desktop_file = true
create_symlink_in_bin = false
</pre>
<p>
Community launchers and scripts can use these hints to configure integration steps (for example, creating a desktop file or a symlink in a user-local bin directory).</p>

<h2>4. Security, isolation, and runtimes</h2>

<h3>4.1 Runtime selection</h3>
<p>
Linux offers flexibility in how runtimes are provided:
</p>
<ul>
  <li><strong>Bundled runtimes</strong> in <code>Resources/Runtimes/</code>, where the app ships its own interpreter or runtime.</li>
  <li><strong>Host-provided runtimes</strong>, where the launcher binds FullApp’s <code>[[runtime]]</code> entries to distro packages or system-level runtimes.</li>
</ul>
<p>
The <code>[[runtime]]</code> entries in <code>FullAppData.toml</code> can include Linux-specific hints to help map them to native packages if available.</p>

<h3>4.2 Isolation approaches</h3>
<p>
Linux launchers may support multiple isolation strategies:
</p>
<ul>
  <li><strong>Direct execution</strong> (no extra sandbox), similar to traditional binaries.</li>
  <li><strong>Namespace-based isolation</strong> using user namespaces, mount namespaces, or containers.</li>
  <li><strong>Integration with existing sandbox frameworks</strong> such as Flatpak, Bubblewrap, or Firejail.</li>
</ul>
<p>
The <code>[security]</code> section in <code>FullAppData.toml</code> informs launchers about what the app expects, so they can choose the appropriate sandbox level or warn the user.</p>

<h2>5. Installation and launch behavior</h2>

<h3>5.1 Simple execution flow</h3>
<p>
A minimal, user-friendly Linux launcher should:
</p>
<ul>
  <li><strong>Validate the archive</strong> (basic structure and required files).</li>
  <li><strong>Optionally extract to a cache or per-user app directory</strong>, or run in-place if desired.</li>
  <li><strong>Set up the environment</strong> (working directory, variables, runtime paths).</li>
  <li><strong>Execute <code>Program.bnry</code></strong> with the correct arguments and environment.</li>
</ul>

<h3>5.2 Desktop integration</h3>
<p>
When configured via metadata, launchers or helper tools can:
</p>
<ul>
  <li><strong>Create a <code>.desktop</code> file</strong> pointing to the FullApp launcher and the specific <code>.fullapp</code> package.</li>
  <li><strong>Install icons</strong> in standard icon directories.</li>
  <li><strong>Register MIME types and URL handlers</strong> based on app metadata.</li>
</ul>
<p>
These steps should respect user choice and avoid requiring root access by default, favoring per-user installation paths.</p>

<h2>6. Community and ecosystem considerations</h2>
<p>
The Easy Linux Adoption Profile encourages:
</p>
<ul>
  <li><strong>Multiple independent launchers</strong> maintained by different communities and distributions.</li>
  <li><strong>Shared tooling</strong> (such as validators, inspectors, and metadata indexers) that work across distros.</li>
  <li><strong>Integration with existing package managers</strong>, either by wrapping FullApp archives or by referencing them in catalogs.</li>
</ul>
<p>
As FullApp evolves, Linux communities can extend this profile with additional desktop-specific or distro-specific guidance while keeping the core behaviors simple and consistent.</p>
