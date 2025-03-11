# DeepDownloader - Multi-Threaded Downloader Library

This library provides a multi-threaded file downloader that divides a file into multiple parts and downloads them concurrently. Once all parts are downloaded, the library merges them into a single file. It also supports saving and loading the download state, so you can resume downloads from where they left off.

## Features

- **Multi-threaded Download**: Split file download into multiple parts for faster speeds.
- **Progress Reporting**: Tracks download progress and state (Downloading, Completed, Mixing).
- **State Saving and Loading**: Save the state of your download to resume later.
- **Range-based Requests**: Downloads different parts of the file using HTTP range requests.

## Getting Started

To use this library, you need to follow these steps:

### Prerequisites

Make sure you have the following prerequisites:

- .NET Framework (or .NET Core/5+)
- `HttpClient` for making HTTP requests (used internally)

### Usage Example

Here’s how to use the `Downloader` class:

#### 1. Create an Instance of the Downloader

```csharp
var downloader = new Downloader("https://example.com/file.zip", "file.zip", 4);
```

- `url`: The URL of the file you want to download.
- `path`: The path where the downloaded file will be saved.
- `taskCount`: The number of concurrent download tasks (parts).

#### 2. Start the Download

```csharp
await downloader.StartAsync();
```

#### 3. Listen for Progress Updates

You can listen for progress updates by subscribing to the `OnProgressChanged` event:

```csharp
downloader.OnProgressChanged += (sender, args) =>
{
    Console.WriteLine($"Progress: {args.DownloadPercentage}% - State: {args.State}");
};
```

#### 4. Stop the Download

If you want to cancel the download:

```csharp
downloader.Stop();
```

#### 5. Save and Load State

You can save the current download state to a file:

```csharp
using (var stream = new FileStream("download_state.txt", FileMode.Create))
{
    downloader.SaveState(stream);
}
```

And later, you can load the state to resume the download:

```csharp
using (var stream = new FileStream("download_state.txt", FileMode.Open))
{
    await downloader.LoadStateAsync(stream);
}
```

### Complete Example

```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

public class Program
{
    public static async Task Main(string[] args)
    {
        var downloader = new Downloader("https://example.com/file.zip", "file.zip", 4);

        downloader.OnProgressChanged += (sender, e) =>
        {
            Console.WriteLine($"Progress: {e.DownloadPercentage}% - State: {e.State}");
        };

        await downloader.StartAsync();
    }
}
```

## Classes and Enums

### `ProgressChangedEventArgs`

This class holds information about the progress of the download.

- `Id`: The ID of the download part.
- `BytesReceived`: The number of bytes downloaded so far for this part.
- `TotalBytes`: The total file size.
- `State`: The current download state.
- `DownloadPercentage`: The percentage of the download completed.

### `DownloadState` Enum

Defines the states of the download:

- `Downloading`: The file is currently being downloaded.
- `Completed`: The download has finished.
- `Mixing`: The downloaded parts are being merged.

### `Downloader` Class

The main class used for downloading the file.

- `StartAsync()`: Starts the download process.
- `Stop()`: Cancels the download.
- `SaveState()`: Saves the current state to a stream.
- `LoadState()`: Loads a previously saved state.

## License

This library is open-source and available under the MIT License.

---

# Biblioteca de Descargador Multihilo

Esta biblioteca proporciona un descargador de archivos multihilo que divide un archivo en varias partes y las descarga de manera concurrente. Una vez que todas las partes están descargadas, la biblioteca las combina en un solo archivo. También admite guardar y cargar el estado de la descarga, por lo que puedes reanudar las descargas desde donde se quedaron.

## Características

- **Descarga Multihilo**: Divide la descarga del archivo en varias partes para obtener mayores velocidades.
- **Informe de Progreso**: Realiza un seguimiento del progreso de la descarga y el estado (Descargando, Completado, Mezclando).
- **Guardar y Cargar Estado**: Guarda el estado de la descarga para poder reanudarla más tarde.
- **Solicitudes basadas en Rango**: Descarga diferentes partes del archivo usando solicitudes HTTP con rangos.

## Comenzando

Para usar esta biblioteca, sigue estos pasos:

### Requisitos Previos

Asegúrate de tener los siguientes requisitos:

- .NET Framework (o .NET Core/5+)
- `HttpClient` para realizar solicitudes HTTP (utilizado internamente)

### Ejemplo de Uso

Aquí te mostramos cómo usar la clase `Downloader`:

#### 1. Crear una Instancia del Downloader

```csharp
var downloader = new Downloader("https://example.com/file.zip", "file.zip", 4);
```

- `url`: La URL del archivo que deseas descargar.
- `path`: La ruta donde se guardará el archivo descargado.
- `taskCount`: El número de tareas de descarga concurrentes (partes).

#### 2. Iniciar la Descarga

```csharp
await downloader.StartAsync();
```

#### 3. Escuchar las Actualizaciones de Progreso

Puedes escuchar las actualizaciones de progreso suscribiéndote al evento `OnProgressChanged`:

```csharp
downloader.OnProgressChanged += (sender, args) =>
{
    Console.WriteLine($"Progreso: {args.DownloadPercentage}% - Estado: {args.State}");
};
```

#### 4. Detener la Descarga

Si deseas cancelar la descarga:

```csharp
downloader.Stop();
```

#### 5. Guardar y Cargar el Estado

Puedes guardar el estado actual de la descarga en un archivo:

```csharp
using (var stream = new FileStream("download_state.txt", FileMode.Create))
{
    downloader.SaveState(stream);
}
```

Y más tarde, puedes cargar el estado para reanudar la descarga:

```csharp
using (var stream = new FileStream("download_state.txt", FileMode.Open))
{
    await downloader.LoadStateAsync(stream);
}
```

### Ejemplo Completo

```csharp
using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.IO;

public class Program
{
    public static async Task Main(string[] args)
    {
        var downloader = new Downloader("https://example.com/file.zip", "file.zip", 4);

        downloader.OnProgressChanged += (sender, e) =>
        {
            Console.WriteLine($"Progreso: {e.DownloadPercentage}% - Estado: {e.State}");
        };

        await downloader.StartAsync();
    }
}
```

## Clases y Enums

### `ProgressChangedEventArgs`

Esta clase contiene la información sobre el progreso de la descarga.

- `Id`: El ID de la parte de la descarga.
- `BytesReceived`: El número de bytes descargados hasta el momento para esta parte.
- `TotalBytes`: El tamaño total del archivo.
- `State`: El estado actual de la descarga.
- `DownloadPercentage`: El porcentaje completado de la descarga.

### Enum `DownloadState`

Define los estados de la descarga:

- `Downloading`: El archivo se está descargando actualmente.
- `Completed`: La descarga ha finalizado.
- `Mixing`: Las partes descargadas se están fusionando.

### Clase `Downloader`

La clase principal utilizada para descargar el archivo.

- `StartAsync()`: Inicia el proceso de descarga.
- `Stop()`: Cancela la descarga.
- `SaveState()`: Guarda el estado actual en un stream.
- `LoadState()`: Carga un estado previamente guardado.

## Licencia

Esta biblioteca es de código abierto y está disponible bajo la licencia MIT.

---

This README provides both English and Spanish instructions to help users understand how to use your library effectively. Feel free to customize it further!
