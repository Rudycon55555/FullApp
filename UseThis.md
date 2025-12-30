<h1>How to use FullApp (and why it matters)</h1>

<h2>1. What FullApp is for</h2>
<p>
FullApp is a portable, self-contained application format designed to feel “native” on every platform while still being simple to inspect, move, and trust. Instead of shipping an installer that mutates the system, a FullApp archive is a single <code>.fullapp</code> file that can be verified, cataloged, and launched by compatible runtimes on macOS, iOS, iPadOS, Windows, Linux, and more.
</p>
<p>
This document explains how different groups can and should use FullApp:
</p>
<ul>
  <li><strong>Platform vendors</strong> (like Apple and Microsoft) who want safer, simpler app distribution.</li>
  <li><strong>Developers</strong> who want one clean package that works across ecosystems.</li>
  <li><strong>Contributors</strong> who want to help FullApp grow (launchers, SDKs, docs, and community).</li>
</ul>

<h2>2. Why platform vendors should adopt FullApp</h2>

<h3>2.1 Native without installers</h3>
<p>
Today, native apps often use complex installers that scatter files across the system, making them hard to audit, sandbox, and remove. FullApp takes the opposite approach: the entire application and its metadata live inside one ZIP-based archive (<code>.fullapp</code>), with a strictly defined directory layout and clear execution model.
</p>
<p>
A platform-level launcher can:
</p>
<ul>
  <li><strong>Treat a FullApp as a first-class app bundle</strong> (like <code>.app</code> on macOS or <code>.exe + manifest</code> on Windows) without running arbitrary installers.</li>
  <li><strong>Inspect the package structurally</strong> using <code>Describe/FullAppData.toml</code> and <code>Describe/Metadata.json</code> before deciding to run it.</li>
  <li><strong>Integrate with system sandboxes</strong> based on declared permissions and security metadata instead of guessing behavior.</li>
</ul>

<h3>2.2 Clear, machine-readable contracts</h3>
<p>
FullApp deliberately separates <strong>code</strong>, <strong>resources</strong>, and <strong>metadata</strong>:
</p>
<ul>
  <li><strong>Program.bnry</strong> is the entry point and is always in a known location.</li>
  <li><strong>Describe/FullAppData.toml</strong> provides a strongly typed manifest that launchers can parse quickly and reliably.</li>
  <li><strong>Describe/Metadata.json</strong> provides store- and catalog-friendly metadata (IDs, versions, links, permissions, integrity, signing hints).</li>
</ul>
<p>
This means a platform vendor can:
</p>
<ul>
  <li><strong>Preflight-check an app</strong> before launch (required files, format version, platform compatibility, permission claims).</li>
  <li><strong>Model app capabilities</strong> (network, filesystem, user input, runtimes) as structured data, mapping them directly to OS-level policy.</li>
  <li><strong>Offer safer defaults</strong> by refusing to launch invalid or malformed packages without guessing or heuristics.</li>
</ul>

<h3>2.3 Respecting existing ecosystems</h3>
<p>
FullApp is not trying to replace everything. It is designed to be:
</p>
<ul>
  <li><strong>One more “native” format</strong> that can live alongside <code>.app</code>, <code>.msix</code>, <code>.apk</code>, etc.</li>
  <li><strong>Friendly to existing runtimes</strong>, because <code>Resources/Runtimes/</code> can embed language runtimes (like .NET, JVM, or others) without forcing a global install.</li>
  <li><strong>Friendly to existing stores</strong>, which can treat FullApp archives as a well-structured payload with clear metadata instead of a black-box installer.</li>
</ul>
<p>
For Apple and Microsoft, adopting FullApp as a supported container type means:
</p>
<ul>
  <li><strong>Less fragile installers</strong>, fewer broken uninstalls, and better user trust.</li>
  <li><strong>Better policy enforcement</strong>, because the platform can reason about the app’s structure and declared capabilities.</li>
  <li><strong>Simpler developer stories</strong>, especially for cross-platform apps that want one consistent packaging format.</li>
</ul>

<h2>3. Why developers should use FullApp</h2>

<h3>3.1 One portable archive, many environments</h3>
<p>
FullApp gives developers a single, well-defined way to bundle an app, its runtimes, and its metadata. By following the spec, the exact same <code>.fullapp</code> file can be:
</p>
<ul>
  <li><strong>Run by different launchers</strong> on different OSes and CPUs, as long as they understand the format.</li>
  <li><strong>Distributed from anywhere</strong> (your own site, GitHub Releases, internal company servers, or app stores).</li>
  <li><strong>Inspected consistently</strong> by tooling that knows how to parse <code>Describe/</code> and verify integrity.</li>
</ul>

<h3>3.2 Freedom to distribute from your own place</h3>
<p>
FullApp is intentionally neutral about distribution. You can:
</p>
<ul>
  <li><strong>Host your apps yourself</strong> (on your own website, static hosting, or internal network).</li>
  <li><strong>Use existing stores</strong> that decide to support FullApp archives as a content format.</li>
  <li><strong>Share directly</strong> (for example, sending a <code>.fullapp</code> file to testers or teammates) without extra installers or scripts.</li>
</ul>
<p>
Because the format is ZIP-based and openly specified, developers and users can:
</p>
<ul>
  <li><strong>Unzip and inspect the app</strong> to see what it declares and how it is structured.</li>
  <li><strong>Build custom tooling</strong> (indexers, scanners, offline catalogs) without guessing the meaning of random files.</li>
</ul>

<h3>3.3 Simple mental model</h3>
<p>
The directory layout is small and predictable:
</p>
<ul>
  <li><strong>Code:</strong> <code>Program.bnry</code> and <code>OtherCode/</code></li>
  <li><strong>Resources and dependencies:</strong> <code>Resources/</code> (including runtimes, static assets, dynamic asset descriptors, and link targets)</li>
  <li><strong>Metadata and documentation:</strong> <code>Describe/</code> (README, metadata, configs, legal)</li>
  <li><strong>Everything else:</strong> <code>Other/</code> for experiments and non-standard data</li>
</ul>
<p>
This makes it easier to:
</p>
<ul>
  <li><strong>Automate packaging</strong> (SDKs and build tools always know where to put things).</li>
  <li><strong>Debug issues</strong> (is the entry path wrong, is a runtime missing, is metadata malformed?).</li>
  <li><strong>Teach new contributors</strong>, who only need to learn a small set of required files and directories.</li>
</ul>

<h2>4. How developers can contribute to FullApp</h2>

<h3>4.1 Build and improve launchers</h3>
<p>
Launchers are the bridge between a <code>.fullapp</code> file and a running application. Helpful contributions include:
</p>
<ul>
  <li><strong>Writing launchers</strong> for different platforms and environments (desktop, server, mobile, sandboxed runtimes).</li>
  <li><strong>Improving validation</strong> (better error messages, strict checks, clear warnings when metadata is inconsistent).</li>
  <li><strong>Integrating with OS features</strong> (file associations, app icons, “Open with FullApp Launcher”, per-app sandboxes).</li>
</ul>
<p>
A polished launcher is often the first thing users see. Good launchers make FullApp feel “native” and trustworthy on every platform they support.</p>

<h3>4.2 Create SDKs and build tooling</h3>
<p>
Developers can also:
</p>
<ul>
  <li><strong>Build SDKs</strong> for popular languages and frameworks that generate correct FullApp archives with minimal configuration.</li>
  <li><strong>Write command-line tools</strong> to pack, sign, verify, and inspect <code>.fullapp</code> files.</li>
  <li><strong>Integrate with existing build systems</strong> (for example, adding a “build FullApp” target to existing projects).</li>
</ul>
<p>
The goal is to make FullApp feel like just another output option: if you can build your app, you can produce a FullApp version with a single command.</p>

<h3>4.3 Grow the community</h3>
<p>
FullApp becomes more valuable as more people use it, teach it, and extend it. Helpful community-focused contributions include:
</p>
<ul>
  <li><strong>Writing guides and tutorials</strong> that explain how to package apps and how launchers work.</li>
  <li><strong>Sharing FullApp-based apps with friends and teammates</strong> so they can experience the format.</li>
  <li><strong>Organizing discussions</strong> (issues, forums, chats) about best practices, security, and platform integration.</li>
  <li><strong>Reviewing and improving the specification</strong>, especially around edge cases, compatibility, and versioning rules.</li>
</ul>

<h2>5. How to use this repository</h2>
<p>
This repository is the home for the FullApp specification and reference materials. You can use it to:
</p>
<ul>
  <li><strong>Read the spec</strong> to understand the directory layout, required files, and execution model.</li>
  <li><strong>Study example packages</strong> (when available) that demonstrate recommended structures and metadata.</li>
  <li><strong>File issues or pull requests</strong> to propose changes, new sections, clarifications, or additional examples.</li>
  <li><strong>Coordinate ecosystem work</strong>, such as launchers, SDKs, and documentation projects that live in other repositories.</li>
</ul>
<p>
As the format evolves, this repository will track the official <code>format_version</code> and document how launchers and tools should handle older and newer versions.</p>

<h2>6. Why this matters long-term</h2>
<p>
FullApp is designed to make applications:
</p>
<ul>
  <li><strong>More portable</strong>, because a single archive can move between systems while keeping structure and meaning.</li>
  <li><strong>More transparent</strong>, because metadata and layout are open and unambiguous.</li>
  <li><strong>More trustworthy</strong>, because integrity and signing metadata are built into the format, not bolted on afterward.</li>
</ul>
<p>
For Apple, Microsoft, and other platform vendors, FullApp is a chance to support a clean, auditable, and sandbox-friendly app format that still feels native. For developers and users, it is a way to distribute and run apps without installing opaque, system-changing installers.
</p>
<p>
If you are reading this, you can help shape how FullApp grows—by experimenting, building tools, giving feedback, and sharing it with others.
</p>
