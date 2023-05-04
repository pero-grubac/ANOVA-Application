using MathNet.Numerics.Distributions;
namespace Lab3
{
    public class Anova
    {
        private int k, n;
        private double[] srVrKolone; //y.j
        private double ukupnaSrVrijednost; //y..
        private int altStepenSlobode;
        private int greskaStepenSlobode;
        private double[] efekat; //alfa_j
        private double SST = 0d;
        private double SSA = 0d;
        private double SSE = 0d;
        private double varijansaSSA;
        private double varijansaSSE;
        private double Sc;
        private double fTest;
        private double tabelaF;
        private double tabelaT;
        private double sumaAltPolja = 0d;
        private double kontrast = 0d;
        private IntervalTuple<double, double> intervalPovjerenja;

        public Anova()
        {
        }

        public Anova(int n, int k)
        {
            this.n = n;
            this.k = k;
            this.srVrKolone = new double[k];
            this.efekat = new double[k];
            this.altStepenSlobode = this.k - 1;
            this.greskaStepenSlobode = this.k * (this.n - 1);
        }

        public int N
        {
            get { return this.n; }
        }

        public int K
        {
            get { return this.k; }
        }

        public double SSA1
        {
            get { return SSA; }
        }

        public double SST1
        {
            get { return SST; }
        }

        public double SSE1
        {
            get { return SSE; }
        }

        public int DfAlt
        {
            get { return altStepenSlobode; }
        }

        public int DfErr
        {
            get { return greskaStepenSlobode; }
        }

        public double F_Test
        {
            get { return fTest; }
        }

        public double F_Table
        {
            get { return tabelaF; }
        }

        public double[] Effect
        {
            get { return efekat; }
        }

        public double Contrat
        {
            get { return kontrast; }
        }

        public double VarSe
        {
            get { return varijansaSSE; }
        }

        public double VarSa
        {
            get { return varijansaSSA; }
        }

        public IntervalTuple<double, double> Interval
        {
            get { return intervalPovjerenja; }
        }

        public void IzracunajSrednjeVrijednostiKolona(double[][] matrix)
        {
            double currSum = 0;
            for (int i = 0; i < K; i++)
            {
                currSum = 0d;
                for (int j = 0; j < N; j++)
                {
                    currSum += matrix[j][i];
                    sumaAltPolja += matrix[j][i];
                }
                srVrKolone[i] = currSum / this.N;
            }
            IzracunajUkupnuSrVrijednost();
        }
        public void IzracunajEfekat(Double[][] matrix)
        {
            for (int i = 0; i < this.k; i++)
                efekat[i] = srVrKolone[i] - ukupnaSrVrijednost;
        }


        public void IzracunajUkupnuSrVrijednost()
        {
            this.ukupnaSrVrijednost = sumaAltPolja / (this.k * this.n);
        }

        public void IzracunajVarijacijeIVarijanse(Double[][] matrix)
        {
            for (int j = 0; j < k; j++)
            {
                for (int i = 0; i < n; i++)
                {
                    SSE += Math.Pow((matrix[i][j] - srVrKolone[j]), 2.0);
                    SST += Math.Pow((matrix[i][j] - ukupnaSrVrijednost), 2.0);
                }
            }
            SSA = SST - SSE;
            varijansaSSA = SSA / altStepenSlobode;
            varijansaSSE = SSE / greskaStepenSlobode;
            fTest = varijansaSSA / varijansaSSE;
        }

        public void IzracunajFTabelarno(int v)
        {
            FisherSnedecor fd = new FisherSnedecor(altStepenSlobode, greskaStepenSlobode);
            double vjerovatnocaTest = 0.000;
            double vjerovatnoca = (double)v * 0.01;
            double korak = 0.001;
            while (vjerovatnoca >= fd.CumulativeDistribution(vjerovatnocaTest))
            {
                vjerovatnocaTest += korak;
            }
            tabelaF = vjerovatnocaTest;
        }

        public void IzracunajStudentovuRaspodjelu(int v, double location, double scale)
        {

            double vjerovatnocaTest = 0.000;
            double vjerovatnoca = (double)v * 0.01;
            double korak = 0.001;
            while (vjerovatnoca >= Student(v, vjerovatnocaTest))
            {
                vjerovatnocaTest += korak;
            }
            tabelaT = vjerovatnocaTest;
        }
        // studnetova preuzeta sa https://learn.microsoft.com/en-us/archive/msdn-magazine/2015/november/test-run-the-t-test-using-csharp
        public static double Gauss(double z)
        {
            // input = z-value (-inf to +inf)
            // output = p under Standard Normal curve from -inf to z
            // e.g., if z = 0.0, function returns 0.5000
            // ACM Algorithm #209
            double y; // 209 scratch variable
            double p; // result. called 'z' in 209
            double w; // 209 scratch variable
            if (z == 0.0)
                p = 0.0;
            else
            {
                y = Math.Abs(z) / 2;
                if (y >= 3.0)
                {
                    p = 1.0;
                }
                else if (y < 1.0)
                {
                    w = y * y;
                    p = ((((((((0.000124818987 * w
                    - 0.001075204047) * w + 0.005198775019) * w
                    - 0.019198292004) * w + 0.059054035642) * w
                    - 0.151968751364) * w + 0.319152932694) * w
                    - 0.531923007300) * w + 0.797884560593) * y * 2.0;
                }
                else
                {
                    y = y - 2.0;
                    p = (((((((((((((-0.000045255659 * y
                    + 0.000152529290) * y - 0.000019538132) * y
                    - 0.000676904986) * y + 0.001390604284) * y
                    - 0.000794620820) * y - 0.002034254874) * y
                    + 0.006549791214) * y - 0.010557625006) * y
                    + 0.011630447319) * y - 0.009279453341) * y
                    + 0.005353579108) * y - 0.002141268741) * y
                    + 0.000535310849) * y + 0.999936657524;
                }
            }
            if (z > 0.0)
                return (p + 1.0) / 2;
            else
                return (1.0 - p) / 2;
        }

        public static double Student(double t, double df)
        {
            // for large integer df or double df
            // adapted from ACM algorithm 395
            // returns 2-tail p-value
            double n = df; // to sync with ACM parameter name
            double a, b, y;
            t = t * t;
            y = t / n;
            b = y + 1.0;
            if (y > 1.0E-6) y = Math.Log(b);
            a = n - 0.5;
            b = 48.0 * a * a;
            y = a * y;
            y = (((((-0.4 * y - 3.3) * y - 24.0) * y - 85.5) /
            (0.8 * y * y + 100.0 + b) + y + 3.0) / b + 1.0) *
            Math.Sqrt(y);
            return 2.0 * Gauss(-y); // ACM algorithm 209
        }
        public void IzracunajKontrast(int a1, int a2)
        {
            kontrast = efekat[a1 - 1] - efekat[a2 - 1];
        }

        private void IzracunajSc()
        {
            double Se = Math.Sqrt(varijansaSSE);
            int kn = this.k * this.n;
            Sc = Se * (double)(Math.Sqrt(2.0 / (double)kn));
        }
        public void formirajInterval()
        {
            IzracunajSc();
            double c1 = 0d, c2 = 0d;
            c1 = kontrast - (tabelaT * Sc);
            c2 = kontrast + (tabelaT * Sc);
            intervalPovjerenja = new IntervalTuple<double, double>(c1, c2);

        }

    }
}

