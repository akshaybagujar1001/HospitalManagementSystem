# Creating Database Migration

Nëse database nuk ka tabelat e reja për Insurance Companies dhe Medications, duhet të krijojmë migration.

## Hapat:

1. Hap terminal në folderin `Backend/HMS.API`

2. Ekzekuto komandën:
```bash
dotnet ef migrations add AddEnterpriseModules --project ../HMS.Infrastructure --startup-project .
```

3. Pastaj aplikoni migration:
```bash
dotnet ef database update --project ../HMS.Infrastructure --startup-project .
```

Ose thjesht restart backend dhe `EnsureCreated()` do të krijojë tabelat automatikisht nëse database është e re.

