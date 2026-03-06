import React, { useState, useEffect, useRef } from 'react';
import * as signalR from '@microsoft/signalr';
import { BellIcon } from '@heroicons/react/24/outline';
import axios from 'axios';
import { useNavigate } from 'react-router-dom';

interface Notification {
    id: number;
    title: string;
    message: string;
    type: string;
    link: string;
    isRead: boolean;
    createdAt: string;
}

const NotificationBell: React.FC = () => {
    const [notifications, setNotifications] = useState<Notification[]>([]);
    const [unreadCount, setUnreadCount] = useState(0);
    const [isOpen, setIsOpen] = useState(false);
    const dropdownRef = useRef<HTMLDivElement>(null);
    const navigate = useNavigate();

    useEffect(() => {
        // Fetch initial notifications
        const fetchNotifications = async () => {
            try {
                const token = localStorage.getItem('token');
                if (!token) return;

                const response = await axios.get('http://localhost:5000/api/notifications', {
                    headers: { Authorization: `Bearer ${token}` }
                });

                if (response.data.success) {
                    setNotifications(response.data.data.items);
                    setUnreadCount(response.data.data.unreadCount);
                }
            } catch (error) {
                console.error('Failed to fetch notifications', error);
            }
        };

        fetchNotifications();

        // Setup SignalR connection
        const token = localStorage.getItem('token');
        if (!token) return;

        const connection = new signalR.HubConnectionBuilder()
            .withUrl("http://localhost:5000/hubs/notifications", {
                accessTokenFactory: () => token
            })
            .withAutomaticReconnect()
            .build();

        connection.on("ReceiveNotification", (notification: Notification) => {
            setNotifications(prev => [notification, ...prev]);
            setUnreadCount(prev => prev + 1);

            // Optional: Play a sound or show a toast notification here
        });

        connection.start().catch(err => console.error("SignalR Connection Error: ", err));

        return () => {
            connection.stop();
        };
    }, []);

    // Close dropdown when clicking outside
    useEffect(() => {
        const handleClickOutside = (event: MouseEvent) => {
            if (dropdownRef.current && !dropdownRef.current.contains(event.target as Node)) {
                setIsOpen(false);
            }
        };

        document.addEventListener("mousedown", handleClickOutside);
        return () => document.removeEventListener("mousedown", handleClickOutside);
    }, []);

    const markAsRead = async (id: number, link: string) => {
        try {
            const token = localStorage.getItem('token');
            await axios.put(`http://localhost:5000/api/notifications/${id}/read`, {}, {
                headers: { Authorization: `Bearer ${token}` }
            });

            setNotifications(prev => prev.map(n => n.id === id ? { ...n, isRead: true } : n));
            setUnreadCount(prev => Math.max(0, prev - 1));
            setIsOpen(false);

            if (link) {
                navigate(link);
            }
        } catch (error) {
            console.error('Failed to mark notification as read', error);
        }
    };

    const markAllAsRead = async () => {
        try {
            const token = localStorage.getItem('token');
            await axios.put(`http://localhost:5000/api/notifications/read-all`, {}, {
                headers: { Authorization: `Bearer ${token}` }
            });

            setNotifications(prev => prev.map(n => ({ ...n, isRead: true })));
            setUnreadCount(0);
        } catch (error) {
            console.error('Failed to mark all as read', error);
        }
    };

    return (
        <div className="relative" ref={dropdownRef}>
            <button
                onClick={() => setIsOpen(!isOpen)}
                className="p-2 text-gray-400 hover:text-gray-500 rounded-full focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-indigo-500 relative"
            >
                <span className="sr-only">View notifications</span>
                <BellIcon className="h-6 w-6" aria-hidden="true" />
                {unreadCount > 0 && (
                    <span className="absolute top-0 right-0 block h-4 w-4 transform -translate-y-1/2 translate-x-1/4 rounded-full bg-red-500 text-white text-[10px] font-bold flex items-center justify-center border-2 border-white ring-2 ring-white">
                        {unreadCount > 99 ? '99+' : unreadCount}
                    </span>
                )}
            </button>

            {isOpen && (
                <div className="origin-top-right absolute right-0 mt-2 w-80 rounded-md shadow-lg bg-white ring-1 ring-black ring-opacity-5 focus:outline-none z-50">
                    <div className="py-2">
                        <div className="px-4 py-2 flex justify-between items-center border-b">
                            <h3 className="text-sm font-semibold text-gray-900">Notifications</h3>
                            {unreadCount > 0 && (
                                <button
                                    onClick={markAllAsRead}
                                    className="text-xs text-indigo-600 hover:text-indigo-900 font-medium"
                                >
                                    Mark all as read
                                </button>
                            )}
                        </div>

                        <div className="max-h-96 overflow-y-auto">
                            {notifications.length === 0 ? (
                                <p className="px-4 py-4 text-sm text-gray-500 text-center">No notifications yet</p>
                            ) : (
                                <ul className="divide-y divide-gray-200">
                                    {notifications.map((notification) => (
                                        <li
                                            key={notification.id}
                                            className={`px-4 py-3 hover:bg-gray-50 cursor-pointer transition duration-150 ease-in-out ${!notification.isRead ? 'bg-blue-50/50' : ''}`}
                                            onClick={() => markAsRead(notification.id, notification.link)}
                                        >
                                            <div className="flex space-x-3">
                                                <div className="flex-1 space-y-1">
                                                    <div className="flex items-center justify-between">
                                                        <h3 className={`text-sm font-medium ${!notification.isRead ? 'text-gray-900' : 'text-gray-600'}`}>
                                                            {notification.title}
                                                        </h3>
                                                        <p className="text-xs text-gray-500">
                                                            {new Date(notification.createdAt).toLocaleDateString()}
                                                        </p>
                                                    </div>
                                                    <p className={`text-sm ${!notification.isRead ? 'text-gray-700' : 'text-gray-500'}`}>
                                                        {notification.message}
                                                    </p>
                                                </div>
                                            </div>
                                        </li>
                                    ))}
                                </ul>
                            )}
                        </div>

                        <div className="px-4 py-2 border-t border-gray-100">
                            <a href="#" className="block text-center text-xs font-medium text-gray-500 hover:text-gray-700">
                                View all notifications
                            </a>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
};

export default NotificationBell;
