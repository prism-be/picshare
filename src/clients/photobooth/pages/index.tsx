import type {NextPage} from 'next'
import React, {useState, useEffect, useRef} from 'react';

import {HttpTransportType, HubConnectionBuilder, LogLevel} from '@microsoft/signalr';

const Home: NextPage = () => {

    const [connection, setConnection] = useState<any>(null);
    const [picture, setPicture] = useState<any>(null);
    const [pictureUrl, setPictureUrl] = useState<any>(null);
    
    var timer: Timeout;

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl(process.env.NEXT_PUBLIC_BACKEND_URL + 'hubs/photobooth')
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
                        
                        clearTimeout(timer);
                        timer = setTimeout(() => { setPictureUrl(null); }, 2 * 60 * 1000);
                    });
                })
                .catch((e: any) => console.log('Connection failed: ', e));
        }
    }, [connection]);

    return (
        <div>
            {pictureUrl && <div className="w-full p-3 h-screen">
                <div className="w-full h-full">
                    <img src={pictureUrl}
                         className="object-contain  w-full h-full"
                         alt={"test"}/>
                </div>
            </div>}
            
            
            {!pictureUrl && <div className="w-full text-center">
                <h1 className="text-5xl p-5 font-bold underline">
                    Bonjour et bienvenue !
                </h1>
                <div>
                    <h2 className={"text-4xl underline"}>Mode d'emploi</h2>
                    <ul className={"text-2xl"}>
                        <li>
                            Positionnez vous au niveau de la ligne au sol
                        </li>
                        <li>
                            Choisissez un volontaire pour prendre la télécommande
                        </li>
                        <li>
                            Souriez et appuyez sur le bouton !
                        </li>
                        <li className={"pt-5"}>
                            La photo reste affichée maximum 2 minutes et le QR code vous permet de la retrouver sur votre téléphone !
                        </li>
                    </ul>
                </div>
                <div className={"flex items-center"}>
                    <img src="/say-cheese.svg" className={"w-1/2 m-auto"} alt={"Say Cheese !"}/>
                </div>
            </div> }

            
        </div>
    )
}

export default Home
