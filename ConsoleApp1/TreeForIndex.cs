using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace SearchSystem
{
    public class TreeForIndex
    {
        private const int BorderA = 1040;
        private const int BorderB = 1071;


        // Корень дерева
        public Sheet Root = new Sheet(BorderA, BorderB);

        public void AddTrigram(string trigram, Guid fileId, int position)
        {

            //Console.WriteLine(trigram);

            // Текущий лист
            Sheet currentSheet = Root;

            int index = 0;
            while(true)
            {
                char symbol = trigram[index];
                // Выбираем направление
                if (symbol <= currentSheet.BorderA + (currentSheet.BorderB - currentSheet.BorderA) / 2)
                {
                    // Влево
                    if ((currentSheet.BorderB - currentSheet.BorderA) / 2 == 0)
                    {
                        //Console.WriteLine("Конец дерева Влево");
                        // Нужно сменить символ
                        index++;
                        if (index == trigram.Length)
                        {
                            



                            break;
                        }

                            




                        //!!!!
                        if (currentSheet.LeftChild == null)
                        {
                            currentSheet.LeftChild = new Sheet(BorderA, BorderB);
                        }



                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                        // Проверяем записан ли текущий документ в списке термина
                        int document_index = currentSheet.LeftChild.Documents.FindIndex(i => i.FileId == fileId);

                        // Если уже есть
                        if (document_index != -1)
                        {
                            // Добавляем позицию термина
                            currentSheet.LeftChild.Documents[document_index].Positions.Add(position);
                            currentSheet.LeftChild.Documents[document_index].Frequency++;
                        }
                        else
                        {
                            // Иначе создаем новый
                            Document _document = new Document()
                            {
                                FileId = fileId,
                                Positions = new List<int>() { position },

                                Frequency = 1
                            };
                            currentSheet.LeftChild.Documents.Add(_document);
                        }













                    }
                    else
                    {
                        if (currentSheet.LeftChild == null)
                        {
                            currentSheet.LeftChild = new Sheet(currentSheet.BorderA, currentSheet.BorderA + (currentSheet.BorderB - currentSheet.BorderA) / 2);
                        }
                    }

                    currentSheet = currentSheet.LeftChild;
                }
                else
                {
                    // Вправо
                    if ((currentSheet.BorderB - currentSheet.BorderA) / 2 == 0)
                    {
                        //Console.WriteLine("Конец дерева Вправо");
                        // Нужно сменить символ
                        index++;
                        if (index == trigram.Length)
                            break;





                        //!!!!!
                        if (currentSheet.RightChild == null)
                        {
                            currentSheet.RightChild = new Sheet(BorderA, BorderB);
                        }
                    }
                    else
                    {
                        if (currentSheet.RightChild == null)
                        {
                            currentSheet.RightChild = new Sheet(currentSheet.BorderA + (currentSheet.BorderB - currentSheet.BorderA) / 2, currentSheet.BorderB);
                        }
                    }

                    currentSheet = currentSheet.RightChild;
                }
            }
        }





























        public void SearchTrigram(string trigram)
        {
            // Текущий лист
            /*Sheet currentSheet = Root;

            int index = 0;
            while (true)
            {
                char symbol = trigram[index];
                // Выбираем направление
                if (symbol <= currentSheet.BorderA + (currentSheet.BorderB - currentSheet.BorderA) / 2)
                {
                    // Влево
                    if ((currentSheet.BorderB - currentSheet.BorderA) / 2 == 0)
                    {
                        //Console.WriteLine("Конец дерева Влево");
                        // Нужно сменить символ
                        index++;
                        if (index == trigram.Length)
                        {




                            break;
                        }






                        //!!!!




                        ///////////////////////////////////////////////////////////////////////////////////////////////////////////
                        // Проверяем записан ли текущий документ в списке термина













                    }
                    else
                    {

                    }

                    currentSheet = currentSheet.LeftChild;
                }
                else
                {
                    // Вправо
                    if ((currentSheet.BorderB - currentSheet.BorderA) / 2 == 0)
                    {
                        //Console.WriteLine("Конец дерева Вправо");
                        // Нужно сменить символ
                        index++;
                        if (index == trigram.Length)
                            break;





                        //!!!!!
                        if (currentSheet.RightChild == null)
                        {
                            currentSheet.RightChild = new Sheet(BorderA, BorderB);
                        }
                    }
                    else
                    {

                    }

                    currentSheet = currentSheet.RightChild;
                }
            }*/
        }















    }


    public class Sheet
    {
        // 
        public string Name { get; set; }
        //
        public int BorderA { get; set; }
        public int BorderB { get; set; }
        public Sheet? LeftChild { get; set; }
        public Sheet? RightChild { get; set; }
        public List<Document> Documents { get; set; }

        public Sheet(int borderA, int borderB, Sheet? leftChild = null, Sheet? rightChild = null)
        {
            Console.WriteLine($"{(char)borderA} ** {(char)borderB}");

            Name = $"{(char)borderA} ** {(char)borderB}";
            BorderA= borderA;
            BorderB= borderB;
            LeftChild = leftChild;
            RightChild = rightChild;
            Documents = new List<Document>();
        }
    }
}
