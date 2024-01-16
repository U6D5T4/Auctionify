import { Component, effect } from '@angular/core';
import { AuthorizeService } from 'src/app/api-authorization/authorize.service';
import { ChatMessage } from 'src/app/models/chats/chat-models';

interface GroupedMessages {
    date: string;
    messages: ChatMessage[];
}

@Component({
    selector: 'app-conversation-window',
    templateUrl: './conversation-window.component.html',
    styleUrls: ['./conversation-window.component.scss'],
})
export class ConversationWindowComponent {
    constructor(private authService: AuthorizeService) {
        effect(() => {
            this.currentUserId = this.authService.getUserId()!;
        });
    }

    currentUserId: number = 0;

    messages: ChatMessage[] = [
        {
            id: 1,
            senderId: 1,
            conversationId: 1,
            body: 'Some text',
            timeStamp: new Date(),
            isRead: true,
        },
        {
            id: 1,
            senderId: 1,
            conversationId: 1,
            body: 'Some text',
            timeStamp: new Date(),
            isRead: true,
        },
        {
            id: 1,
            senderId: 3,
            conversationId: 1,
            body: 'Some text',
            timeStamp: new Date(),
            isRead: true,
        },
        {
            id: 1,
            senderId: 3,
            conversationId: 1,
            body: 'Some text',
            timeStamp: new Date(),
            isRead: true,
        },
        {
            id: 1,
            senderId: 3,
            conversationId: 1,
            body: 'Some text',
            timeStamp: new Date(),
            isRead: true,
        },
        {
            id: 1,
            senderId: 3,
            conversationId: 1,
            body: 'Some text',
            timeStamp: new Date(),
            isRead: true,
        },
        {
            id: 1,
            senderId: 3,
            conversationId: 1,
            body: 'Some text',
            timeStamp: new Date(),
            isRead: true,
        },
        {
            id: 1,
            senderId: 3,
            conversationId: 1,
            body: 'Some text',
            timeStamp: new Date(),
            isRead: true,
        },
        {
            id: 1,
            senderId: 3,
            conversationId: 1,
            body: 'Some text',
            timeStamp: new Date(),
            isRead: true,
        },
        {
            id: 1,
            senderId: 1,
            conversationId: 1,
            body: 'Some big text Some big text Some big text Some big text Some big text Some big text Some big text ',
            timeStamp: new Date(),
            isRead: true,
        },
        {
            id: 1,
            senderId: 3,
            conversationId: 1,
            body: 'Some big text Some big text Some big text Some big text Some big text Some big text Some big text ',
            timeStamp: new Date(),
            isRead: true,
        },
        {
            id: 1,
            senderId: 3,
            conversationId: 1,
            body: 'Some big text Some big text Some big text Some big text Some big text Some big text Some big text ',
            timeStamp: new Date(2024, 0, 15),
            isRead: true,
        },
        {
            id: 1,
            senderId: 3,
            conversationId: 1,
            body: 'Some big text Some big text Some big text Some big text Some big text Some big text Some big text ',
            timeStamp: new Date(2024, 0, 15),
            isRead: true,
        },
    ];

    groupMessagesByDate(messages: ChatMessage[]): GroupedMessages[] {
        const groupedMessages: GroupedMessages[] = [];

        messages.forEach((message) => {
            const dateKey = message.timeStamp.toLocaleDateString();

            const existingGroup = groupedMessages.find(
                (group) => group.date === dateKey
            );

            if (existingGroup) {
                existingGroup.messages.push(message);
            } else {
                groupedMessages.push({ date: dateKey, messages: [message] });
            }
        });

        groupedMessages.sort(
            (a, b) => new Date(a.date).getTime() - new Date(b.date).getTime()
        );

        return groupedMessages;
    }

    generateMessageClass(message: ChatMessage) {
        if (this.currentUserId == message.senderId) {
            return 'self';
        } else return 'other';
    }
}
