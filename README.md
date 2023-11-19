# GitHub Actions Usage Analyzer

This console application parses a GitHub Metered Billing report and provides a summary of the usage of GitHub Actions across your organizations. The summary includes the total number of minutes used, the number of minutes used per repository, and the total cost of GitHub Actions for each organization.

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

## Report output example

<details>
<summary>Expand this secction to see this example</summary>

```txt

Actions SKUs for this enterprise
================================

SKU                      | Price per minute | Multiplier
------------------------ | ---------------- | ----------
Compute - UBUNTU         | $0.01            | 1,0       
Compute - UBUNTU_4_CORE  | $0.02            | 1,0       
Compute - UBUNTU_16_CORE | $0.06            | 1,0       
Compute - WINDOWS        | $0.02            | 2,0       
Compute - UBUNTU_8_CORE  | $0.03            | 1,0       
Compute - WINDOWS_8_CORE | $0.06            | 2,0       
Compute - MACOS          | $0.08            | 10,0      


Total number of organizations: 6

Actions consumption per organization
====================================

owner-316
---------

Consumption per SKU
-------------------

SKU                      | Minutes   | Total price
------------------------ | --------- | -----------
Compute - UBUNTU         | 361.196,0 | $2,889.57  
Compute - UBUNTU_4_CORE  | 13.908,0  | $222.53    
Compute - UBUNTU_16_CORE | 10,0      | $0.64      
Compute - WINDOWS        | 21.672,0  | $693.50    
Compute - UBUNTU_8_CORE  | 26.387,0  | $844.38    
Compute - WINDOWS_8_CORE | 2.101,0   | $268.93    
Compute - MACOS          | 5.541,0   | $4,432.80  


Total cost for this organization: $9,352.35

Top 3 repositories by consumption
---------------------------------

Repository | Total price
---------- | -----------
repo-76    | $1,674.18  
repo-696   | $1,017.06  
repo-782   | $851.37    


owner-879
---------

Consumption per SKU
-------------------

SKU                     | Minutes   | Total price
----------------------- | --------- | -----------
Compute - UBUNTU        | 148.831,0 | $1,190.65  
Compute - UBUNTU_8_CORE | 183,0     | $5.86      
Compute - MACOS         | 576,0     | $460.80    


Total cost for this organization: $1,657.30

Top 3 repositories by consumption
---------------------------------

Repository | Total price
---------- | -----------
repo-93    | $473.65    
repo-696   | $207.19    
repo-670   | $196.22    


owner-88
--------

Consumption per SKU
-------------------

SKU               | Minutes   | Total price
----------------- | --------- | -----------
Compute - UBUNTU  | 143.706,0 | $1,149.65  
Compute - WINDOWS | 11.198,0  | $358.34    
Compute - MACOS   | 14.911,0  | $11,928.80 


Total cost for this organization: $13,436.78

Top 3 repositories by consumption
---------------------------------

Repository | Total price
---------- | -----------
repo-554   | $11,988.55 
repo-376   | $271.70    
repo-477   | $160.76    


owner-659
---------

Consumption per SKU
-------------------

SKU               | Minutes | Total price
----------------- | ------- | -----------
Compute - UBUNTU  | 8.605,0 | $68.84     
Compute - WINDOWS | 2.301,0 | $73.63     
Compute - MACOS   | 234,0   | $187.20    


Total cost for this organization: $329.67

Top 3 repositories by consumption
---------------------------------

Repository | Total price
---------- | -----------
repo-102   | $141.07    
repo-326   | $73.63     
repo-140   | $52.61     


owner-182
---------

Consumption per SKU
-------------------

SKU              | Minutes | Total price
---------------- | ------- | -----------
Compute - UBUNTU | 832,0   | $6.66      


Total cost for this organization: $6.66

Top 3 repositories by consumption
---------------------------------

Repository | Total price
---------- | -----------
repo-124   | $4.60      
repo-401   | $1.02      
repo-771   | $0.58      


owner-303
---------

Consumption per SKU
-------------------

SKU                      | Minutes   | Total price
------------------------ | --------- | -----------
Compute - UBUNTU         | 545.347,0 | $4,362.78  
Compute - UBUNTU_4_CORE  | 30.539,0  | $488.62    
Compute - WINDOWS        | 34.900,0  | $1,116.80  
Compute - UBUNTU_8_CORE  | 46.542,0  | $1,489.34  
Compute - WINDOWS_8_CORE | 3.396,0   | $434.69    
Compute - MACOS          | 5.546,0   | $4,436.80  


Total cost for this organization: $12,329.03

Top 3 repositories by consumption
---------------------------------

Repository | Total price
---------- | -----------
repo-76    | $2,862.45  
repo-696   | $831.04    
repo-93    | $644.03    


Total consumption for the enterprise: $37,111.80

```

</details>




## Contributing

If you find a bug or have a feature request, please [open an issue](https://github.com/asizikov/github-actions-usage-analyzer/issues/new). Pull requests are also welcome!

## License

This project is licensed under the [MIT License](LICENSE).