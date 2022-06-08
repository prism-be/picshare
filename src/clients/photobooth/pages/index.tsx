import type {NextPage} from 'next'
import React, {useEffect, useState} from 'react';

import {HubConnectionBuilder, LogLevel} from '@microsoft/signalr';
import QRCodeSVG from "qrcode.react";

const Home: NextPage = () => {

    const [connection, setConnection] = useState<any>(null);
    const [pictureUrl, setPictureUrl] = useState<any>(null);
    const [frontPictureUrl, setFrontPictureUrl] = useState<any>(null);

    let timer: any;

    useEffect(() => {
        const newConnection = new HubConnectionBuilder()
            .withUrl(process.env.NEXT_PUBLIC_BACKEND_URL + '/hubs/photobooth')
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

                    connection.on('PictureTaken', (photoboothPicture: any) => {
                        console.log(photoboothPicture);
                        setPictureUrl(process.env.NEXT_PUBLIC_BACKEND_URL + "/pictures/" + photoboothPicture.id)
                        clearTimeout(timer);
                        timer = setTimeout(() => {
                            setPictureUrl(null);
                            setFrontPictureUrl(null);
                        }, 2 * 60 * 1000);
                    });

                    connection.on('PictureUploaded', (photoboothPicture: any) => {
                        console.log(photoboothPicture);
                        setFrontPictureUrl(process.env.NEXT_PUBLIC_FRONT_URL + "/taken/" + photoboothPicture.sessionId + "/" + photoboothPicture.id);
                        clearTimeout(timer);
                        timer = setTimeout(() => {
                            setPictureUrl(null);
                            setFrontPictureUrl(null);
                        }, 2 * 60 * 1000);
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
                {frontPictureUrl && <div className={"absolute top-5 left-5"}>
                    <QRCodeSVG size={128} value={frontPictureUrl}/>
                </div>
                }
            </div>}


            {!pictureUrl && <div className="w-full text-center">
                <h1 className="text-5xl p-5 font-bold underline">
                    Bonjour et bienvenue !
                </h1>
                <div>
                    <h2 className={"text-4xl underline"}>Mode d'emploi</h2>
                    <ul className={"text-2xl pt-5"}>
                        <li>
                            Positionnez vous au niveau de la ligne au sol
                        </li>
                        <li>
                            Choisissez un volontaire pour prendre la télécommande
                        </li>
                        <li>
                            Souriez et appuyez sur le bouton !
                        </li>
                        <li className={"pt-5 italic text-xl"}>
                            La photo reste affichée maximum 2 minutes et le QR code vous permet de la retrouver sur
                            votre téléphone.
                        </li>
                    </ul>
                </div>
                <div className={"flex items-center"}>
                    <img src="/say-cheese.svg" className={"w-96 m-auto"} alt={"Say Cheese !"}/>
                </div>
                <div className={"text-center text-xl italic"}>
                    Les photos que vous prenez seront envoyées à Hadrien et Laurie après le mariage, n'hésitez pas à
                    leur laisser un souvenir !
                </div>

            </div>}


        </div>
    )
}

export default Home
