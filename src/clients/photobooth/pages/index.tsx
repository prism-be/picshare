import type { NextPage } from 'next'
import React, { useState, useEffect, useRef } from 'react';

import { HubConnectionBuilder } from '@microsoft/signalr';

import Head from 'next/head'
import Image from 'next/image'
import styles from '../styles/Home.module.css'

const Home: NextPage = () => {

    const [ connection, setConnection ] = useState<any>(null);
    const [picture, setPicture] = useState<any>(null);
    const [pictureUrl, setPictureUrl] = useState<any>(null);

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl('http://localhost:5047/hubs/photobooth')
            .withAutomaticReconnect()
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        if (connection) {
            connection.start()
                .then(() => {
                    console.log('Connected!');

                    connection.on('PictureTaken', (pictureTaken: any) => {
                        console.log(pictureTaken);
                        setPicture(pictureTaken);
                        setPictureUrl("http://localhost:5047/pictures/" + pictureTaken.id)
                    });
                })
                .catch((e: any) => console.log('Connection failed: ', e));
        }
    }, [connection]);
    
  return (
    <div>
        {pictureUrl && <img src={pictureUrl} alt={picture.originalFileName} />}
    </div>
  )
}

export default Home
