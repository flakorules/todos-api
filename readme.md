# Todos - API

Código fuente de una api que expone las funcionalidades para administrar los registros de una aplicación de Todos, la cual se construyó utilizando .NET 5, C#, Entity Framework y PostgreSQL.

## Instrucciones para levantar el ambiente de desarrollo

### Pre requisitos

* Visual Studio 2019 o superior
* Acceso a una insstancia de PostgreSQL

### Paso a paso

1. Descargar el codigo fuente utilizando el siguiente comando.

   ```
   git clone https://github.com/flakorules/todos-api.git
   ```
2. Modificar **ConnectionString -> TodosDB** en **\todos.api\appsettings.json**, reeplazando **"USUARIO"**, **"PASSWORD"**, **"SERVIDOR"**, **"PUERTO"** y **"BASE DE DATOS"** correspondientes a tu instancia de PostgreSQL.

   ```
     "ConnectionString": {
       "TodosDB": "User ID=<USUARIO>;Password=<PASSWORD>;Host=<SERVIDOR>;Port=<PUERTO>;Database=<BASE DE DATOS>;Pooling=true;SSL Mode=Require;Trust Server Certificate=True;"
     }
   ```
3. Ejecutar el siguiente comando desde directorio **\todos-api**, el cual desplegará en la instancia PostgreSQL la estructura de tablas que soportará el mantenedor. Al igual que en el paso anterior, se deben reemplazar los datos **"USUARIO"**, **"PASSWORD"**, **"SERVIDOR"**, **"PUERTO"** y **"BASE DE DATOS"** correspondientes a tu instancia de PostgreSQL.

   ```
   dotnet ef database update --project todos.api --connection "User ID=<USUARIO>;Password=<PASSWORD>;Host=<SERVIDOR>;Port=<PUERTO>;Database=<BASE DE DATOS>;Pooling=true;SSL Mode=Require;Trust Server Certificate=True;"
   ```

   Se puede revisar la correcta ejecución de la creación de las tablas desde PgAdmin utilizando las credenciales de tu instancia PostgreSQL


   ![](assets/20220623_171428_PgAdmin.PNG)
