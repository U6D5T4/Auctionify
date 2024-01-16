export interface ChatItem {
    imageUrl: string;
    userName: string;
    messageText: string;
    unread: string | undefined;
}

export interface Conversation {
    id: number;
    lotId: number;
    chatUser: ChatUser;
    lastMessage: ChatMessage;
    unreadMessagesCount: number;
}

export interface ChatUser {
    id: number;
    fullName: string;
    profilePhoto: string;
}

export interface ChatMessage {
    id: number;
    senderId: number;
    conversationId: number;
    body: string;
    timeStamp: string;
    isRead: boolean;
}
