# Security Policy

This policy covers security maintenance for the `v1.3.0` release distributed through GitHub Releases and explains how to report suspected vulnerabilities in the Personal Log Manager Client.

## 📑 Table of Contents

- [Supported Versions](#-supported-versions)
- [Reporting a Vulnerability](#-reporting-a-vulnerability)
- [Scope](#-scope)
- [Disclosure Policy](#-disclosure-policy)

## 🛡️ Supported Versions

Use this table to indicate which project versions currently receive security maintenance.

| Version | Distribution Channel | Supported |
|---------|--------------------|-----------|
| Latest version | GitHub Releases | ✅ |
| Preceding versions | Any distribution channel | ❌ |

## 🚨 Reporting a Vulnerability

Please do not disclose suspected vulnerabilities publicly before maintainers have had an opportunity to validate and remediate them.

To report a vulnerability:
- [GitHub Security Advisories](https://github.com/hmlendea/personal-log-manager-client/security/advisories)
- Contact the maintainers directly

## 📌 Scope

The subsequent report categories are in scope for this repository:
- API-key storage, exposure, or handling within the client
- Unauthorised access, disclosure, or mutation of personal log data caused by the client

The subsequent categories are out of scope unless explicitly stated to the contrary:
- Vulnerabilities in the upstream Personal Log Manager API or other external services
- Vulnerabilities limited to a user's local operating system, browser, network, or deployment environment

## 📢 Disclosure Policy

This project follows coordinated disclosure:
1. Vulnerabilities are investigated privately.
2. A remediation plan is prepared and validated.
3. Public disclosure is published after a fix, mitigation, or agreed risk decision is available.
4. Credit is attributed in accordance with reporter preference and project policy.
