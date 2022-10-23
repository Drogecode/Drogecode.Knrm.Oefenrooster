namespace Drogecode.Knrm.Oefenrooster.Client.Models;

public record NotificationAuthor(string DisplayName, string AvatarUlr);
public record NotificationMessage(string Id, string Title, string Except, string Category, DateTime PublishDate, string ImgUrl, IEnumerable<NotificationAuthor> Authors, Type ContentComponent);
