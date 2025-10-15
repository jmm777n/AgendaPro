using System;
using System.Collections.Generic;
using System.Globalization;

namespace AgendaPro
{
    internal class Persona
    {
        public int Id { get; set; }
        public string Nombre { get; set; } = "";
        public string Telefono { get; set; } = "";

        public Persona(int id, string nombre, string telefono)
        {
            Id = id;
            Nombre = nombre;
            Telefono = telefono;
        }
    }

    internal class Cita
    {
        public int PersonaId { get; set; }
        public DateTime Fecha { get; set; }
        public string Descripcion { get; set; } = "";
    }

    internal class Program
    {
        static readonly List<Persona> personas = new();
        static readonly List<Cita> citas = new();

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            string opcion;
            do
            {
                MostrarMenu();
                opcion = (Console.ReadLine() ?? "").Trim().ToLower();

                switch (opcion)
                {
                    case "a":
                        RegistrarPersona();
                        break;
                    case "b":
                        ListarPersonas();
                        break;
                    case "c":
                        CrearCita();
                        break;
                    case "d":
                        ListarCitasPorPersona();
                        break;
                    case "e":
                        ListarTodasLasCitas();
                        break;
                    case "f":
                        Console.WriteLine("Saliendo... ¡Hasta luego!");
                        break;
                    default:
                        Console.WriteLine("Opción inválida. Intente nuevamente.");
                        break;
                }

                if (opcion != "f")
                {
                    Console.WriteLine("\nPresione una tecla para continuar...");
                    Console.ReadKey();
                    Console.Clear();
                }

            } while (opcion != "f");
        }

        // ------------------ Menú ------------------
        static void MostrarMenu()
        {
            Console.WriteLine("==== AgendaPro - Gestión de Personas y Citas ====");
            Console.WriteLine("a) Registrar persona (Id único)");
            Console.WriteLine("b) Listar personas");
            Console.WriteLine("c) Crear cita para una persona");
            Console.WriteLine("d) Listar citas por PersonaId");
            Console.WriteLine("e) Mostrar todas las citas");
            Console.WriteLine("f) Salir");
            Console.Write("Seleccione una opción: ");
        }

        // ------------------ Opciones ------------------
        static void RegistrarPersona()
        {
            Console.WriteLine("\n-- Registrar persona --");

            int id = LeerEntero("Id (entero): ");

            if (personas.Exists(p => p.Id == id))
            {
                Console.WriteLine("❌ Ya existe una persona con ese Id. Intente nuevamente.");
                return;
            }

            string nombre = LeerTextoNoVacio("Nombre: ");
            string telefono = LeerTextoNoVacio("Teléfono: ");

            personas.Add(new Persona(id, nombre, telefono));
            Console.WriteLine("✅ Persona registrada correctamente.");
        }

        static void ListarPersonas()
        {
            Console.WriteLine("\n-- Lista de personas --");

            if (personas.Count == 0)
            {
                Console.WriteLine("No hay personas registradas.");
                return;
            }

            Console.WriteLine("{0,6} | {1,-25} | {2,-15}", "Id", "Nombre", "Teléfono");
            Console.WriteLine(new string('-', 52));
            foreach (var p in personas)
                Console.WriteLine("{0,6} | {1,-25} | {2,-15}", p.Id, p.Nombre, p.Telefono);
        }

        static void CrearCita()
        {
            Console.WriteLine("\n-- Crear cita --");

            int personaId = LeerEntero("PersonaId: ");

            var persona = personas.Find(p => p.Id == personaId);
            if (persona == null)
            {
                Console.WriteLine("❌ No existe una persona con ese Id.");
                return;
            }

            DateTime fecha = LeerFecha("Fecha y hora (ej. 2025-10-15 14:30): ");
            string descripcion = LeerTextoNoVacio("Descripción: ");

            citas.Add(new Cita
            {
                PersonaId = personaId,
                Fecha = fecha,
                Descripcion = descripcion
            });

            Console.WriteLine($"✅ Cita creada para {persona.Nombre} en {fecha}.");
        }

        static void ListarCitasPorPersona()
        {
            Console.WriteLine("\n-- Listar citas por PersonaId --");
            int personaId = LeerEntero("PersonaId: ");

            var persona = personas.Find(p => p.Id == personaId);
            if (persona == null)
            {
                Console.WriteLine("❌ No existe una persona con ese Id.");
                return;
            }

            var citasPersona = citas.FindAll(c => c.PersonaId == personaId);
            if (citasPersona.Count == 0)
            {
                Console.WriteLine("No hay citas registradas para esta persona.");
                return;
            }

            Console.WriteLine("{0,9} | {1,-19} | {2,-40}", "PersonaId", "Fecha", "Descripción");
            Console.WriteLine(new string('-', 74));
            foreach (var c in citasPersona)
                Console.WriteLine("{0,9} | {1,-19} | {2,-40}",
                    c.PersonaId,
                    c.Fecha.ToString("yyyy-MM-dd HH:mm"),
                    c.Descripcion);
        }

        static void ListarTodasLasCitas()
        {
            Console.WriteLine("\n-- Todas las citas --");
            if (citas.Count == 0)
            {
                Console.WriteLine("No hay citas registradas.");
                return;
            }

            Console.WriteLine("{0,9} | {1,-19} | {2,-40}", "PersonaId", "Fecha", "Descripción");
            Console.WriteLine(new string('-', 74));
            foreach (var c in citas)
                Console.WriteLine("{0,9} | {1,-19} | {2,-40}",
                    c.PersonaId,
                    c.Fecha.ToString("yyyy-MM-dd HH:mm"),
                    c.Descripcion);
        }

        // ------------------ Helpers de entrada ------------------
        static int LeerEntero(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var texto = Console.ReadLine();
                try
                {
                    if (texto is null) throw new FormatException();
                    return int.Parse(texto.Trim());
                }
                catch (FormatException)
                {
                    Console.WriteLine("Entrada inválida. Ingrese un número entero (ej.: 10).");
                }
                catch (OverflowException)
                {
                    Console.WriteLine("Número fuera de rango para int.");
                }
            }
        }

        static DateTime LeerFecha(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var texto = Console.ReadLine();
                try
                {
                    if (texto is null) throw new FormatException();

                    // Acepta: "yyyy-MM-dd HH:mm", "yyyy/MM/dd HH:mm", y formatos locales
                    var formatos = new[]
                    {
                        "yyyy-MM-dd HH:mm", "yyyy/MM/dd HH:mm",
                        "dd/MM/yyyy HH:mm", "dd-MM-yyyy HH:mm",
                        "yyyy-MM-dd", "dd/MM/yyyy", "dd-MM-yyyy"
                    };

                    if (DateTime.TryParseExact(texto.Trim(), formatos,
                            CultureInfo.CurrentCulture, DateTimeStyles.None, out var dt))
                        return dt;

                    if (DateTime.TryParse(texto.Trim(), out dt))
                        return dt;

                    throw new FormatException();
                }
                catch (FormatException)
                {
                    Console.WriteLine("Fecha inválida. Ejemplos: 2025-10-15 14:30 | 15/10/2025 14:30");
                }
            }
        }

        static string LeerTextoNoVacio(string prompt)
        {
            while (true)
            {
                Console.Write(prompt);
                var t = (Console.ReadLine() ?? "").Trim();
                if (!string.IsNullOrWhiteSpace(t))
                    return t;

                Console.WriteLine("El campo no puede estar vacío.");
            }
        }
    }
}
