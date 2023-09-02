# Repository Name

This repository contains [description of what the repository is for].

## Usage

To use this repository, follow these steps:

1. Clone the repository to your local machine.
2. [Add any additional steps for setting up the repository, if necessary.]
3. [Add any additional steps for running the code or using the repository, if necessary.]

## Viewing Your GitHub Actions Usage

To view your GitHub Actions usage, follow these steps:

https://docs.github.com/en/enterprise-cloud@latest/billing/managing-billing-for-github-actions/viewing-your-github-actions-usage

1. Go to the "Settings" tab of your enterprise account.
2. Under "Billing", click "Get usage report".
3. Select the date range for the report and wait for the report to generate.

## Test this tool

```
docker run -v $(pwd)/examples/input.csv:/input myimage --input input.csv
```

## Report output example

```txt
Actions SKUs for this enterprise:
Compute - UBUNTU - $0.01 per minute, multiplier: 1,0
Compute - UBUNTU_4_CORE - $0.02 per minute, multiplier: 1,0
Compute - UBUNTU_16_CORE - $0.06 per minute, multiplier: 1,0
Compute - WINDOWS - $0.02 per minute, multiplier: 2,0
Compute - UBUNTU_8_CORE - $0.03 per minute, multiplier: 1,0
Compute - WINDOWS_8_CORE - $0.06 per minute, multiplier: 2,0
Compute - MACOS - $0.08 per minute, multiplier: 10,0

Total number of organizations: 6 


Owner: org-name-one

Consumption per SKU:
SKU: Compute - UBUNTU - 361.196,0 minutes, total price: $2,889.57
SKU: Compute - UBUNTU_4_CORE - 13.908,0 minutes, total price: $222.53
SKU: Compute - UBUNTU_16_CORE - 10,0 minutes, total price: $0.64
SKU: Compute - WINDOWS - 21.672,0 minutes, total price: $693.50
SKU: Compute - UBUNTU_8_CORE - 26.387,0 minutes, total price: $844.38
SKU: Compute - WINDOWS_8_CORE - 2.101,0 minutes, total price: $268.93
SKU: Compute - MACOS - 5.541,0 minutes, total price: $4,432.80

Top 3 repositories by consumption:
frontend-repo : $1,674.18
frontend-repo-02 : $1,017.06
frontend-repo-03 : $851.37

------------------
Total cost for this owner: $9,352.35
======================================

Owner: org-name-two

Consumption per SKU:
SKU: Compute - UBUNTU - 148.831,0 minutes, total price: $1,190.65
SKU: Compute - UBUNTU_8_CORE - 183,0 minutes, total price: $5.86
SKU: Compute - MACOS - 576,0 minutes, total price: $460.80

Top 3 repositories by consumption:
frontend-repo-04 : $473.65
frontend-repo-05 : $196.22
frontend-repo-06 : $194.90

------------------
Total cost for this owner: $1,657.30
======================================

Owner: org-name-three

Consumption per SKU:
SKU: Compute - UBUNTU - 143.706,0 minutes, total price: $1,149.65
SKU: Compute - WINDOWS - 11.198,0 minutes, total price: $358.34
SKU: Compute - MACOS - 14.911,0 minutes, total price: $11,928.80

Top 3 repositories by consumption:
frontend-repo-07 : $11,988.06
devops-repo-01 : $269.73
python-repo-01 : $160.30

------------------
Total cost for this owner: $13,436.78
======================================

Owner: org-name-four

Consumption per SKU:
SKU: Compute - UBUNTU - 8.605,0 minutes, total price: $68.84
SKU: Compute - WINDOWS - 2.301,0 minutes, total price: $73.63
SKU: Compute - MACOS - 234,0 minutes, total price: $187.20

Top 3 repositories by consumption:
Retail-PIM-Test : $141.07
devops-automation-01 : $73.63
devops-automation-02 : $52.61

------------------
Total cost for this owner: $329.67
======================================

Owner: org-name-five

Consumption per SKU:
SKU: Compute - UBUNTU - 832,0 minutes, total price: $6.66

Top 3 repositories by consumption:
data-engeering-01 : $4.60
devops-common-01 : $1.02
backend-repo-02 : $0.58

------------------
Total cost for this owner: $6.66
======================================

Owner: org-name-six

Consumption per SKU:
SKU: Compute - UBUNTU - 545.347,0 minutes, total price: $4,362.78
SKU: Compute - UBUNTU_4_CORE - 30.539,0 minutes, total price: $488.62
SKU: Compute - WINDOWS - 34.900,0 minutes, total price: $1,116.80
SKU: Compute - UBUNTU_8_CORE - 46.542,0 minutes, total price: $1,489.34
SKU: Compute - WINDOWS_8_CORE - 3.396,0 minutes, total price: $434.69
SKU: Compute - MACOS - 5.546,0 minutes, total price: $4,436.80

Top 3 repositories by consumption:
frontend-repo : $2,862.45
frontend-repo-02 : $831.04
website : $643.30

------------------
Total cost for this owner: $12,329.03
======================================
Total consumption for the enterprise: $37,111.80
```
