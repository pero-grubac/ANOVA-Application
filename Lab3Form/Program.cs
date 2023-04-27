using Lab3;

namespace Lab3Form
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Anova a = new Anova(3, 5);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            StartForm startForm = new StartForm();

            Application.Run(startForm);
        }
    }
}
