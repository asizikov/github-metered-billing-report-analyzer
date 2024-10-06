# GitHub Actions Usage Analyzer

This console application parses a GitHub Metered Billing report and provides a summary of the usage of GitHub Actions across your organizations. The summary detailes information about metered producs that has been consumed by your Enteprise with a breakdown of the consumption per organization.

## Installation

To use this application, you need to have [.NET](https://dotnet.microsoft.com/download) and optinally Docker installed on your machine. 

## Usage

To use this repository, follow these steps:

1. Clone the repository to your local machine.
2. `dotnet build`
3. `dotnet test`

## Viewing Your GitHub Actions Usage

To view your GitHub Actions usage, follow these steps:

https://docs.github.com/en/enterprise-cloud@latest/billing/managing-billing-for-github-actions/viewing-your-github-actions-usage

1. Go to the "Settings" tab of your enterprise account.
2. Under "Billing", click "Get usage report".
3. Select the date range for the report and wait for the report to generate.

Once you have the report, you can run the application using the following command:

```bash
docker run -b $(pwd)/path-to-your-report:/input -v $(pwd)/output:/output analyzer --input your-usage-report.csv
```

The application will parse the report and provide a summary of the usage of GitHub Actions in your organization.

## Test this tool

```bash
docker run -v $(pwd)/examples:/input -v $(pwd)/output:/output analyzer --input input.csv  gold.md
```

## Using the latest release

To use the latest release, follow these steps:

```bash
docker run -v $(pwd)/examples:/input -v $(pwd)/output:/output ghcr.io/asizikov/github-metered-billing-report-analyzer:main --input input.csv  output.md
```

## Report output example

<details>
<summary>Expand this secction to see this example</summary>

```txt

## Metered SKUs for this enterprise

Metered data for period: **2023-08-01** to **2023-08-26**
| Product | SKU | Unit | Price per unit |
| --- | --- | --- | --- |
| Actions | Compute - UBUNTU | minute | $0.01 |
| Actions | Compute - UBUNTU_4_CORE | minute | $0.02 |
| Actions | Compute - WINDOWS | minute | $0.02 |
| Actions | Compute - MACOS | minute | $0.08 |
| Actions | Compute - MACOS_XLARGE | minute | $0.16 |
| CodespacesLinux | Prebuild storage | gb-month | $0.07 |
| CodespacesLinux | Storage | gb-month | $0.07 |
| CodespacesLinux | Compute - 2 core | hour | $0.18 |
| CodespacesLinux | Compute - 4 core | hour | $0.36 |
| Copilot | Copilot Business | user-month | $19.00 |
| Packages | Data Transfer | gb | $0.50 |
| SharedStorage | Shared Storage | gb-day | $0.01 |

Total number of organizations: 5

## Actions consumption per organization


### owner-316


#### Consumption per SKU

| SKU | Minutes | Total cost |
| --- | --- | --- |
| Compute - UBUNTU | 5,058.0 | $40.46 |
| Compute - UBUNTU_4_CORE | 25.0 | $0.40 |

Total cost for this organization: $40.86

#### Top 3 repositories by consumption

| Repository | Total cost |
| --- | --- |
| repo-604 | $39.92 |
| repo-499 | $0.12 |
| repo-657 | $0.12 |


### owner-879


#### Consumption per SKU

| SKU | Minutes | Total cost |
| --- | --- | --- |
| Compute - UBUNTU | 148.0 | $1.18 |

Total cost for this organization: $1.18

#### Top 3 repositories by consumption

| Repository | Total cost |
| --- | --- |
| repo-714 | $1.18 |


### owner-88


#### Consumption per SKU

| SKU | Minutes | Total cost |
| --- | --- | --- |
| Compute - UBUNTU | 14.0 | $0.11 |

Total cost for this organization: $0.11

#### Top 3 repositories by consumption

| Repository | Total cost |
| --- | --- |
| repo-61 | $0.05 |
| repo-52 | $0.03 |
| repo-983 | $0.02 |


### owner-659


#### Consumption per SKU

| SKU | Minutes | Total cost |
| --- | --- | --- |
| Compute - UBUNTU | 1.0 | $0.01 |

Total cost for this organization: $0.01

#### Top 3 repositories by consumption

| Repository | Total cost |
| --- | --- |
| repo-818 | $0.01 |


### owner-152


#### Consumption per SKU

| SKU | Minutes | Total cost |
| --- | --- | --- |
| Compute - WINDOWS | 19.0 | $0.30 |
| Compute - MACOS | 481.0 | $38.48 |
| Compute - MACOS_XLARGE | 11.0 | $1.76 |

Total cost for this organization: $40.54

#### Top 3 repositories by consumption

| Repository | Total cost |
| --- | --- |
| repo-374 | $40.24 |
| repo-682 | $0.30 |

Total consumption for the enterprise: $82.71

## Shared storage consumption per organization


### owner-152


#### Top 3 repositories by storage cost

| Repository | Total price |
| --- | --- |
| repo-444 | $0.42 |
| repo-348 | $0.04 |
| repo-235 | $0.01 |

Total storage cost for this organization: $0.50

Total storage consumption for the enterprise: $0.50

## Packages consumption per organization


### owner-152


#### Top 3 sources by packages cost

| Source | Total cost |
| --- | --- |
| repo-863 | $0.16 |
| repo-248 | $0.00 |
| repo-111 | $0.00 |

Total packages cost for this organization: $0.16

Total packages consumption for the enterprise: $0.16

## Codespaces consumption per organization


### owner-152


#### Consumption per SKU

| SKU | Unit | Total cost |
| --- | --- | --- |
| Prebuild storage | gb-month | $0.39 |
| Storage | gb-month | $0.07 |
| Compute - 4 core | hour | $1.56 |
| Compute - 2 core | hour | $0.25 |


#### Top 3 repositories by codespaces cost

| Repository | Total cost |
| --- | --- |
| repo-508 | $1.57 |
| repo-180 | $0.26 |
| repo-366 | $0.21 |

Total codespaces cost for this organization: $2.27

Total codespaces consumption for the enterprise: $2.27

## Copilot consumption per organization


### owner-152

Total copilot cost for this organization: $519.74

Total copilot consumption for the enterprise: $519.74

# Summary for this enterprise

| Metered Cost | Total price |
| --- | --- |
| Actions | $82.71 |
| SharedStorage | $0.50 |
| Packages | $0.16 |
| CodespacesLinux | $2.27 |
| Copilot | $519.74 |

Grand total: $605.39

```

</details>




## Contributing

If you find a bug or have a feature request, please [open an issue](https://github.com/your-username/github-actions-usage-analyzer/issues/new). Pull requests are also welcome!

## License

This project is licensed under the [MIT License](LICENSE).