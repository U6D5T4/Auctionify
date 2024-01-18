import { Component, OnInit } from '@angular/core';
import { Conversation } from 'src/app/models/chats/chat-models';
import { Client } from 'src/app/web-api-client';

@Component({
    selector: 'app-chat-page',
    templateUrl: './chat-page.component.html',
    styleUrls: ['./chat-page.component.scss'],
})
export class ChatPageComponent implements OnInit {
    constructor(private client: Client) {}
    ngOnInit(): void {
        this.client.getAllUserConversations().subscribe({
            next: (result) => {
                this.chatConversations = result.conversations;
            },
        });
    }
    chosenConversationId: number = 0;
    chatConversations: Conversation[] = [];
    // chatConversations: Conversation[] = [
    //     {
    //         id: 1,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 2,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 3,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    //     {
    //         id: 4,
    //         lotId: 1,
    //         chatUser: {
    //             id: 1,
    //             fullName: 'Anastasia M',
    //             profilePhoto:
    //                 'https://buffer.com/cdn-cgi/image/w=1000,fit=contain,q=90,f=auto/library/content/images/size/w1200/2023/10/free-images.jpg',
    //         },
    //         lastMessage: {
    //             id: 23,
    //             senderId: 1,
    //             conversationId: 1,
    //             body: "Hi there! I'm interested in the a...",
    //             timeStamp: new Date(),
    //             isRead: true,
    //         },
    //         unreadMessagesCount: 20,
    //         isActive: false,
    //     },
    // ];

    handleConversationChange(id: number) {
        this.chosenConversationId = id;
    }
}
