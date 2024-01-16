import { Component } from '@angular/core';
import { Conversation } from 'src/app/models/chats/chat-models';

@Component({
    selector: 'app-chat-page',
    templateUrl: './chat-page.component.html',
    styleUrls: ['./chat-page.component.scss'],
})
export class ChatPageComponent {
    chosenConversationId: number = 0;
    chatConversations: Conversation[] = [
        {
            id: 1,
            lotId: 1,
            chatUser: {
                id: 1,
                fullName: 'Anastasia M',
                profilePhoto:
                    'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
            },
            lastMessage: {
                id: 23,
                senderId: 1,
                conversationId: 1,
                body: "Hi there! I'm interested in the a...",
                timeStamp: '09:26 PM',
                isRead: true,
            },
            unreadMessagesCount: 20,
        },
        {
            id: 1,
            lotId: 1,
            chatUser: {
                id: 1,
                fullName: 'Anastasia M',
                profilePhoto:
                    'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
            },
            lastMessage: {
                id: 23,
                senderId: 1,
                conversationId: 1,
                body: "Hi there! I'm interested in the a...",
                timeStamp: '09:26 PM',
                isRead: true,
            },
            unreadMessagesCount: 20,
        },
        {
            id: 1,
            lotId: 1,
            chatUser: {
                id: 1,
                fullName: 'Anastasia M',
                profilePhoto:
                    'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
            },
            lastMessage: {
                id: 23,
                senderId: 1,
                conversationId: 1,
                body: "Hi there! I'm interested in the a...",
                timeStamp: '09:26 PM',
                isRead: true,
            },
            unreadMessagesCount: 20,
        },
        {
            id: 1,
            lotId: 1,
            chatUser: {
                id: 1,
                fullName: 'Anastasia M',
                profilePhoto:
                    'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
            },
            lastMessage: {
                id: 23,
                senderId: 1,
                conversationId: 1,
                body: "Hi there! I'm interested in the a...",
                timeStamp: '09:26 PM',
                isRead: true,
            },
            unreadMessagesCount: 20,
        },
    ];

    handleConversationChange(id: number) {
        this.chosenConversationId = id;
    }
}
