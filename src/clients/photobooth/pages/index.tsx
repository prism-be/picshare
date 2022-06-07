import type { NextPage } from 'next'
import React, { useState, useEffect, useRef } from 'react';

import {HttpTransportType, HubConnectionBuilder, LogLevel} from '@microsoft/signalr';

const Home: NextPage = () => {

    const [ connection, setConnection ] = useState<any>(null);
    const [picture, setPicture] = useState<any>(null);
    const [pictureUrl, setPictureUrl] = useState<any>(null);

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl( process.env.NEXT_PUBLIC_BACKEND_URL + 'hubs/photobooth')
            .configureLogging(LogLevel.Information)
            .build();

        setConnection(newConnection);
    }, []);

    useEffect(() => {
        
        if (connection) {

            console.log("Initiate Connection to hub ...");
            
            connection.start()
                .then(() => {
                    console.log('Connected!');

                    connection.on('PictureTaken', (pictureTaken: any) => {
                        console.log(pictureTaken);
                        setPicture(pictureTaken);
                        setPictureUrl(process.env.NEXT_PUBLIC_BACKEND_URL + "pictures/" + pictureTaken.id)
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
