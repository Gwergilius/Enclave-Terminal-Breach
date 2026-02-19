# Forráskód

**[English]** | Magyar

Ebben a mappában az Enclave Terminal Breach projekt teljes forráskódja található, **platform** szerint, hogy a .NET, Python és TypeScript implementációk együtt legyenek.

## Mappa szerkezet

| Mappa | Tartalom |
|--------|----------|
| **dotnet/** | [.NET implementáció](dotnet/README.hu.md) – C# solution (Common, Core, SPARROW, tesztek). A solutiont innen nyitod, a buildet **src/dotnet/**-ból futtatod. |
| **excel-prototype/** | Excel/VBA prototípus (pre-SPARROW); nem része egyetlen solutionnek sem. |
| **python/** | *Tervezett.* Python implementáció. |
| **typescript/** | *Tervezett.* TypeScript implementáció. |

## Gyors indulás (.NET)

A repó gyökeréből:

```powershell
cd src/dotnet
dotnet build Enclave.Echelon.slnx
```

A solution szerkezetéhez, konfigurációhoz és coverage-hoz lásd a [dotnet README](dotnet/README.hu.md)-t.

[English]: ./README.md
