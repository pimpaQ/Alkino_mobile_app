using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MauiApp1.VKApi
{
    public class VkPost
    {
        public string Text { get; set; }
        public string ImageUrl { get; set; }
        public string PostUrl { get; set; }

        // Новое свойство для отображения первого абзаца с учетом длины
        public string FirstParagraph
        {
            get
            {
                if (string.IsNullOrEmpty(Text))
                    return string.Empty;

                // Разделяем текст на абзацы
                var paragraphs = Text.Split('\n', StringSplitOptions.RemoveEmptyEntries);

                if (paragraphs.Length == 0)
                    return string.Empty;

                // Проверяем длину первого абзаца
                var firstParagraph = paragraphs[0];

                // Если первый абзац короче 20 символов
                if (firstParagraph.Length < 40)
                {
                    // Добавляем второй абзац, если он существует
                    if (paragraphs.Length > 1)
                    {
                        firstParagraph += " " + paragraphs[1];
                    }

                    // Добавляем третий абзац, если первый и второй абзацы все еще короткие
                    if (firstParagraph.Length < 100 && paragraphs.Length > 2)
                    {
                        firstParagraph += " " + paragraphs[2];
                    }
                }

                return firstParagraph.Trim();
            }
        }

    }

}
