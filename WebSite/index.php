<?php
/*
 * CopyRight MARTINEAU Emeric (C) 2008
 *
 * This program is free software; you can redistribute it and/or modify it under
 * the terms of the GNU General Public License as published by the Free Software
 * Foundation; either version 3 of the License, or (at your option) any later
 * version.
 *
 * This program is distributed in the hope that it will be useful, but WITHOUT
 * ANY WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS
 * FOR A PARTICULAR PURPOSE.See the GNU GENERAL PUBLIC LICENSE for more
 * details.
 *
 * You should have received a copy of the GNU GENERAL PUBLIC LICENSE along
 * with this program; if not, write to the Free Software Foundation, Inc., 59
 * Temple Place, Suite 330, Boston, MA 02111-1307 USA.
 */
$francais = false;
$english = false;
$changelog = "Changelog" ;
$lang = "english" ;

if (isset($_GET["lang"]) == false)
{
	$languepreferee = explode(',',$_SERVER['HTTP_ACCEPT_LANGUAGE']);

	if ($languepreferee[0] == 'fr')
	{
		$francais = true ;
		$changelog = "Historique";
		$lang  = "francais" ;
	}
	elseif ($languepreferee[0] == 'de')
	{
	}
	elseif ($languepreferee[0] == 'es')
	{
	}
	else
	{
		$english = true ;	
	}
}
else
{
    if ($_GET["lang"] == "francais")
	{
	    $francais = true ;
		$changelog = "Historique";
		$lang  = "francais" ;
	}
	else
	{
	    $english = true ;
	}
}

function numero_de_version($texte)
{
    $resultat = '' ;

    $nb = strlen($texte) ;
    
    for($j = $nb; $j > 0; $j--)
    {
        if ($texte[$j] == '-')
        {
            $j++ ;
            break ;
        }
    }				 
	
    for($k = $nb; $k > 0; $k--)
    {
        if ($texte[$k] == '.')
        {
            $k++ ;
            break ;
        }
    }
    
    for ($i = $j; $i < $k; $i++)
    {
        $resultat .= $texte[$i] ;
    }

    return $resultat ;
}

function lister($dir)
{
    $fd = opendir($dir) ;

    if ($fd)
    {
        $tab_fichier = array() ;

        while($fichier = readdir($fd))
        {
            if (!ereg("^\.", $fichier))
            {
                $tab_fichier[count($tab_fichier)] = $fichier ;
            }
        }

        rsort($tab_fichier) ;

        for ($i = 0; $i < count($tab_fichier); $i++)
        {
            echo "<li><a href='" . $dir . $tab_fichier[$i] . "'>" . numero_de_version($tab_fichier[$i]) . " [" .  date ("d/m/Y H:i:s", filemtime($dir . $tab_fichier[$i])) . "]</a></li>" ;
        }
    }			 
}

function listerOS($dir, $os)
{
    $fd = opendir($dir) ;

    if ($fd)
    {
        $tab_fichier = array() ;

        while($fichier = readdir($fd))
        {
            if ((!ereg("^\.", $fichier)) && (strpos(strtolower($fichier), $os) !== false))
            {
                $tab_fichier[count($tab_fichier)] = $fichier ;
            }
        }

        rsort($tab_fichier) ;

        for ($i = 0; $i < count($tab_fichier); $i++)
        {
            echo "<li><a href='" . $dir . $tab_fichier[$i] . "'>" . numero_de_version($tab_fichier[$i]) . " [" .  date ("d/m/Y H:i:s", filemtime($dir . $tab_fichier[$i])) . "]</a></li>" ;
        }
    }			 
}	

function listerLanguage($dir)
{
    $fd = opendir($dir) ;

    if ($fd)
    {
        $tab_fichier = array() ;

        while($fichier = readdir($fd))
        {
            if (!ereg("^\.", $fichier))
            {
                $tab_fichier[count($tab_fichier)] = $fichier ;
            }
        }

        rsort($tab_fichier) ;

        for ($i = 0; $i < count($tab_fichier); $i++)
        {
            echo "<li><a href='" . $dir . $tab_fichier[$i] . "'>" ;
			
			if (strpos(strtolower($tab_fichier[$i]), "-en-") !== false)
			{
				echo "(English) " ;
			}
			else if (strpos(strtolower($tab_fichier[$i]), "-fr-") !== false)
			{
				echo "(Fran&ccedil;ais) " ;
			}
			
			echo numero_de_version($tab_fichier[$i]) . " [" .  date ("d/m/Y H:i:s", filemtime($dir . $tab_fichier[$i])) . "]</a></li>" ;
        }
    }			 
}
?>
<!DOCTYPE html PUBLIC "-//W3C//DTD HTML 4.01//EN" "http://www.w3.org/TR/html4/strict.dtd">
<html>
  <head>
    <meta content="text/html; charset=ISO-8859-1" http-equiv="content-type">
	<title>MicroFTPServer/LeechDotNet Project</title>
	<link rel="stylesheet" type="text/css" media="all" href="style.css" />
  </head>
  <body>
    <p>
<?php 
    if ($francais == true)
	{
	    echo '<a href="?lang=english">English version</a>';
	}
	else
	{
	    echo '<a href="?lang=francais">Version fran&ccedil;aise</a>';
	}
?>	
    </p>
    <h1>MicroFTPServer</h1>
  
    <p class="description">
	   <?php
	   if ($english == true)
	   {
	   ?>
	   MicroFTPServer is full FTP server in dotNet technologie. It's writting in C# with <a href="http://www.microsoft.com/express/">Visual Studio 2008 Express</a>.<br />
       It can be run under Microsoft Windows with Framework .Net 2.0 SP1 or later (it can be easily compile for other framework 3.0, 3.5 ...).<br />
       You can run it under Unix, MacOS (and Windows) with <a href="http://www.mono-project.com/Main_Page">Mono Project</a>.
	   <?php
	   }
	   else
	   {
	   ?>
	   MicroFTPServer est un server FTP complet en technologie .Net. Il est écrit en C# avec <a href="http://www.microsoft.com/express/">Visual Studio 2008 Express</a>.<br />
	   Il peut fonctionner sous Microsoft Windows avec le framework .Net 2.0 SP1 ou supérieur (il peut être facilement compilé pour d'autre framework 3.0, 3.5...).<br />
	   Il peut, grâce à <a href="http://www.mono-project.com/Main_Page">Mono Project</a> être éxécuté sous Unix et MacOS.
	   <?php
	   }?>
       <br />
	   <br />
	   <?php
	   if ($english == true)
	   {
	   ?>	
	   MicroFTPServer is only free (under <a href="http://www.gnu.org/licenses/gpl.html">GNU GPL license</a>) FTP Server in .Net 2.0 or later.<br />
       It's very lite and simple configuration (just one ini file to server and one ini file per user).<br />
       It can be use for professionnal or personnal use.
	   <?php
	   }
	   else
	   {
	   ?>
	   MicroFTPServer est le seul serveur FTP, sous .Net 2.0 et suivant, libre (<a href="http://www.gnu.org/licenses/gpl.html">licence GNU GPL</a>) et gratuit<br />
	   Il est légé et a une configuration simple (juste un fichier ini de configuration et un fichier ini par utilisateur).<br />
       Il est destiné à un usage professionel ou personnel.
	   <?php
	   }?>	   
	   <br />
	   <br />
       <a href="server-ftp/historique.txt"><?php echo $changelog ; ?></a>
	</p>  
  
    <h1>LeechDotNet</h1>
	
	<p class="description">
	   <?php
	   if ($english == true)
	   {
	   ?>	
	   LeechDotNet is FTP client writting in C#.<br />
	   It can be run under Microsoft Windows with Framework .Net 2.0 SP1 or later (it can be easily compile for other framework 3.0, 3.5 ...).<br />
	   WARRING : you cannot run it with Mono. For this time, i don't know why.
	   <?php
	   }
	   else
	   {
	   ?>
	   LeechDotNet est un client FTP écrit en C#.<br />
	   Il peut fonctionner sous Microsoft Windows avec le framework .Net 2.0 SP1 ou supérieur (il peut être facilement compilé pour d'autre framework 3.0, 3.5...).<br />
	   ATTENTION : pour une raison inconnue, il ne peut pas fonctionné avec Mono.
	   <?php
	   }?>	   
	   <br />
	   <br />
       <a href="client-ftp/historique.txt"><?php echo $changelog ; ?></a></p>
  
  
    <h1>Download</h1>
  
    <p class="niveau1">
      <h2>MicroFTPServer</h2>
	    <h3>Source</h3>
	      <ul>
          <?php
	          lister("server-ftp/src/") ;
          ?>
          </ul>	  	  	  
	  
        <h3>Windows Binary</h2>
	      <ul>
          <?php
			 listerOS("server-ftp/bin/", "windows") ;
          ?>
          </ul>	  	  
	    
        <h3>Unix Binary</h3>
	      <ul>
          <?php
			 listerOS("server-ftp/bin/", "unix") ;
           ?>
          </ul>	 

      <h2>LeechDoNet (FTP Client)</h2>
	    <h3>Source</h3>
	      <ul>
          <?php
	          lister("client-ftp/src/") ;
          ?>
          </ul>	  	  	  
	  
        <h3>Windows Binary</h2>
	      <ul>
          <?php
			 listerLanguage("client-ftp/bin/") ;
          ?>
          </ul>			  
    </p>
	


<p>Copyright &copy; MARTINEAU Emeric 2008 and later - <a href="http://www.mono-project.com/Main_Page">Design Mono Project</a></p>
</body></html>