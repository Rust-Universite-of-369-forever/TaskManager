using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Text.Encodings.Web;
using TaskManager.Models;

namespace TaskManager.Helpers;

public static class TaskHtmlHelper
{
    public static IHtmlContent PriorityBadge(
        this IHtmlHelper html, string priority)
    {
        var text = HtmlEncoder.Default.Encode(priority);

        var css = priority switch
        {
            "Высокий" => "badge bg-danger",
            "Средний" => "badge bg-warning text-dark",
            _ => "badge bg-success"
        };

        return new HtmlString($"<span class=\"{css}\">{text}</span>");
    }

    public static IHtmlContent DueDateBadge(
        this IHtmlHelper html, TaskItem task)
    {
        if (!task.IsOverdue)
            return new HtmlString("");

        return new HtmlString(
            "<span class=\"badge bg-danger ms-1\">Просрочено</span>");
    }
}
