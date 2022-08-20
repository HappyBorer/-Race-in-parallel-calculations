const int THREADS_NUMBER = 4;
const int N = 100000;
object locker = new object();


Random rand = new Random();
int[] array = new int[N].Select(r => rand.Next(5)).ToArray();

int[] resParallel = new int[N];
Array.Copy(array, resParallel, N);


CountingSortExtended(array);

PrepareParallelCountingSort(resParallel);
Console.WriteLine(EqualityMatrix(array, resParallel));

void PrepareParallelCountingSort(int[] inputArray)
{
    int max = inputArray.Max();
    int min = inputArray.Min();
    int offset = -min;
    int[] counters = new int[max + offset + 1];

    int eachThreadCalc = N / THREADS_NUMBER;
    var threadParallel = new List<Thread>();
    for (int i = 0; i < THREADS_NUMBER; i++)
    {
        int startPos = i * eachThreadCalc;
        int endPos = (i + 1) * eachThreadCalc;
        if (i == THREADS_NUMBER - 1) endPos = N;
        threadParallel.Add(new Thread(() => CountingSortParallel(inputArray, counters, offset, startPos, endPos)));
        threadParallel[i].Start();
    }

    foreach (var t in threadParallel)
    {
        t.Join();
    }

    int index = 0;
    for (int i = 0; i < counters.Length; i++)
    {
        for (int j = 0; j < counters[i]; j++)
        {
            inputArray[index] = i - offset;
            index++;   
        }
    }
}

void CountingSortParallel(int[] inputArray, int[] counters, int offset, int startPos, int endPos)
{
    for (int i = startPos; i < endPos; i++)
    {
        counters[inputArray[i] + offset]++;
    }

}



void CountingSortExtended(int[] inputArray)
{
    int max = inputArray.Max();
    int min = inputArray.Min();

    int offset = -min;
    int[] counters = new int[max + offset + 1];
    for (int i = 0; i < inputArray.Length; i++)
    {
        counters[inputArray[i] + offset]++;
    }

    int index = 0;
    for (int i = 0; i < counters.Length; i++)
    {
        for (int j = 0; j < counters[i]; j++)
        {
            inputArray[index] = i - offset;
            index++;   
        }
    }
}

bool EqualityMatrix(int[] fmatrix, int[] smatrix)
{
    bool res = true;
    for (int i = 0; i < N; i++)
    {
        res = res && (fmatrix[i] == smatrix[i]);
    }
    return res;
}


